using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Aeon.Ephys;
using Bonsai;
using Bonsai.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenEphys.Onix1;

namespace Aeon.Tests
{
    [TestClass]
    public class GainCalibrationTests
    {
        const string SerialA = "12345678";
        const string SerialB = "23456789";

        string root = string.Empty;

        [TestInitialize]
        public void CreateCalibrationRoot()
        {
            root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(root);
        }

        [TestCleanup]
        public void DeleteCalibrationRoot()
        {
            Directory.Delete(root, true);
        }

        string CreateCalibrationFile(string serial)
        {
            var folder = Path.Combine(root, serial);
            Directory.CreateDirectory(folder);
            var fileName = Path.Combine(folder, serial + "_gainCalValues.csv");
            File.WriteAllText(fileName, serial);
            return fileName;
        }

        static ImplantedSubject CreateRecord(params (string Probe, string Serial)[] implants)
        {
            var record = new ImplantedSubject { Subject = "BA-12345" };
            foreach (var (probe, serial) in implants)
            {
                record.Implants.Add(new ProbeImplant
                {
                    Probe = probe,
                    SerialNumber = serial,
                    TargetArea = "CA1",
                    InsertionNumber = record.Implants.Count + 1
                });
            }

            return record;
        }

        EphysConfiguration CreateConfiguration(string headstageType, bool enableProbeB = true)
        {
            // The configuration is read from YAML, as a rig reads it. The test covers the file shape too.
            var yaml = string.Join("\n",
                "subject: BA-12345",
                "calibrationRoot: '" + root + "'",
                "headstage:",
                "  type: " + headstageType,
                "  port: PortB");
            if (headstageType != "Headstage64Configuration")
            {
                yaml += "\n  probeB:\n    enable: " + (enableProbeB ? "true" : "false");
            }

            return SurgeryRecordTests.Deserialize<EphysConfiguration>(yaml);
        }

        // Placeholders such as <serial> are not legal path characters on .NET Framework.
        // The expected message paths are therefore joined by hand.
        static string FormatFileName(string folder, string serial)
        {
            var separator = Path.DirectorySeparatorChar;
            return folder + separator + serial + separator + serial + "_gainCalValues.csv";
        }

        static EphysConfiguration ResolveConfiguration(EphysConfiguration configuration, ImplantedSubject record)
        {
            return new ResolveGainCalibration()
                .Process(Observable.Return(Tuple.Create(configuration, record)))
                .Wait();
        }

        static Headstage Resolve(EphysConfiguration configuration, ImplantedSubject record)
        {
            return ResolveConfiguration(configuration, record).Headstage;
        }

        [Combinator]
        class ResolutionSource
        {
            public IObservable<Tuple<EphysConfiguration, ImplantedSubject>> Process()
            {
                return Observable.Empty<Tuple<EphysConfiguration, ImplantedSubject>>();
            }
        }

        [Combinator]
        class ContextSource
        {
            public IObservable<ContextTask> Process()
            {
                return Observable.Empty<ContextTask>();
            }
        }

        static string GainCalibrationFileName(ConfigureNeuropixelsV2PsbDecoder probe)
        {
            return ((NeuropixelsV2QuadShankProbeConfiguration)probe.ProbeConfiguration).GainCalibrationFileName;
        }

        static string GainCalibrationFileName(ConfigureNeuropixelsV2BetaPsbDecoder probe)
        {
            return ((NeuropixelsV2QuadShankProbeConfiguration)probe.ProbeConfiguration).GainCalibrationFileName;
        }

        [TestMethod]
        public void MatchingSerialResolvesFileOnDevices()
        {
            // Each enabled probe gets the file named by the serial in its own implant entry.
            // The resolved copy still derives device identity from the port.
            var fileA = CreateCalibrationFile(SerialA);
            var fileB = CreateCalibrationFile(SerialB);
            var configuration = CreateConfiguration("NeuropixelsV2eConfiguration");
            var record = CreateRecord(("ProbeB", SerialB), ("ProbeA", SerialA));

            var headstage = (ConfigureHeadstageNeuropixelsV2e)(NeuropixelsV2eConfiguration)Resolve(configuration, record);

            Assert.AreEqual(Path.Combine(root, SerialA, SerialA + "_gainCalValues.csv"), fileA);
            Assert.AreEqual(fileA, GainCalibrationFileName(headstage.NeuropixelsV2A));
            Assert.AreEqual(fileB, GainCalibrationFileName(headstage.NeuropixelsV2B));
            Assert.AreEqual(512u, headstage.NeuropixelsV2A.DeviceAddress);
            Assert.AreEqual(513u, headstage.NeuropixelsV2B.DeviceAddress);
            Assert.AreEqual("HeadstageNeuropixelsV2e/NeuropixelsV2B", headstage.NeuropixelsV2B.DeviceName);
        }

        [TestMethod]
        public void ResolvedFileIsWrittenToJson()
        {
            // The session metadata records the resolved path. The package JSON serialiser
            // writes it under the electrode settings of each enabled probe.
            var fileA = CreateCalibrationFile(SerialA);
            var configuration = CreateConfiguration("NeuropixelsV2eConfiguration", enableProbeB: false);
            var resolved = ResolveConfiguration(configuration, CreateRecord(("ProbeA", SerialA)));

            var json = Newtonsoft.Json.Linq.JObject.Parse(
                new SerializeToJson().Process(Observable.Return(resolved)).Wait());

            Assert.AreEqual("NeuropixelsV2eConfiguration", (string?)json.SelectToken("headstage.type"));
            Assert.AreEqual(
                fileA,
                (string?)json.SelectToken("headstage.probeA.probeConfiguration.gainCalibrationFileName"));
            Assert.AreEqual(
                string.Empty,
                (string?)json.SelectToken("headstage.probeB.probeConfiguration.gainCalibrationFileName"));
        }

        [TestMethod]
        public void ResolvedConfigurationKeepsOtherSettings()
        {
            // The resolved configuration is what the workflow publishes. Only the headstage
            // differs from the configuration read from the file, which is left unchanged.
            var fileA = CreateCalibrationFile(SerialA);
            var configuration = CreateConfiguration("NeuropixelsV2eConfiguration", enableProbeB: false);

            var resolved = ResolveConfiguration(configuration, CreateRecord(("ProbeA", SerialA)));

            Assert.AreNotSame(configuration, resolved);
            Assert.AreEqual(configuration.Subject, resolved.Subject);
            Assert.AreEqual(configuration.CalibrationRoot, resolved.CalibrationRoot);
            Assert.AreSame(configuration.BreakoutBoard, resolved.BreakoutBoard);
            var resolvedProbe = ((NeuropixelsV2eConfiguration)resolved.Headstage).ProbeA;
            var originalProbe = ((NeuropixelsV2eConfiguration)configuration.Headstage).ProbeA;
            Assert.AreEqual(fileA, resolvedProbe.ProbeConfiguration.GainCalibrationFileName);
            Assert.AreEqual(string.Empty, originalProbe.ProbeConfiguration.GainCalibrationFileName);
        }

        [TestMethod]
        public void ResolvedConfigurationBuildsDeviceOperator()
        {
            // Mirrors the workflow. The resolved configuration feeds the headstage selection.
            // A single property mapping carries the headstage to the device operator.
            var workflow = new ExpressionBuilderGraph();
            var context = workflow.Add(new CombinatorBuilder { Combinator = new ContextSource() });
            var source = workflow.Add(new CombinatorBuilder { Combinator = new ResolutionSource() });
            var resolve = workflow.Add(new CombinatorBuilder { Combinator = new ResolveGainCalibration() });
            var headstage = workflow.Add(new MemberSelectorBuilder { Selector = nameof(EphysConfiguration.Headstage) });
            var match = workflow.Add(new MatchHeadstage { Type = new TypeMapping<NeuropixelsV2eConfiguration>() });
            var mapping = workflow.Add(new PropertyMappingBuilder
            {
                PropertyMappings =
                {
                    new PropertyMapping { Name = nameof(ConfigureNeuropixelsV2eDevices.Configuration) }
                }
            });
            var devices = workflow.Add(new CombinatorBuilder { Combinator = new ConfigureNeuropixelsV2eDevices() });
            workflow.AddEdge(source, resolve, new ExpressionBuilderArgument());
            workflow.AddEdge(resolve, headstage, new ExpressionBuilderArgument());
            workflow.AddEdge(headstage, match, new ExpressionBuilderArgument());
            workflow.AddEdge(match, mapping, new ExpressionBuilderArgument());
            workflow.AddEdge(mapping, devices, new ExpressionBuilderArgument());
            workflow.AddEdge(context, devices, new ExpressionBuilderArgument());

            Assert.IsNotNull(workflow.Build());
        }

        [TestMethod]
        public void MatchingSerialResolvesFileOnBetaDevices()
        {
            var fileA = CreateCalibrationFile(SerialA);
            var configuration = CreateConfiguration("NeuropixelsV2eBetaConfiguration", enableProbeB: false);
            var record = CreateRecord(("ProbeA", SerialA));

            var headstage = (ConfigureHeadstageNeuropixelsV2eBeta)(NeuropixelsV2eBetaConfiguration)Resolve(configuration, record);

            Assert.AreEqual(fileA, GainCalibrationFileName(headstage.NeuropixelsV2A));
            Assert.AreEqual(string.Empty, GainCalibrationFileName(headstage.NeuropixelsV2B));
            Assert.AreEqual(513u, headstage.NeuropixelsV2B.DeviceAddress);
        }

        [TestMethod]
        public void ResolutionLeavesConfigurationUnchanged()
        {
            // The configuration is shared through a subject. Resolution must work on a copy.
            CreateCalibrationFile(SerialA);
            CreateCalibrationFile(SerialB);
            var configuration = CreateConfiguration("NeuropixelsV2eConfiguration");

            Resolve(configuration, CreateRecord(("ProbeA", SerialA), ("ProbeB", SerialB)));

            var original = (ConfigureHeadstageNeuropixelsV2e)(NeuropixelsV2eConfiguration)configuration.Headstage;
            Assert.AreEqual(string.Empty, GainCalibrationFileName(original.NeuropixelsV2A));
        }

        [TestMethod]
        public void DisabledProbeIsNotResolved()
        {
            // A disabled probe needs neither an implant entry nor a calibration file.
            CreateCalibrationFile(SerialA);
            var configuration = CreateConfiguration("NeuropixelsV2eConfiguration", enableProbeB: false);

            var headstage = (ConfigureHeadstageNeuropixelsV2e)(NeuropixelsV2eConfiguration)Resolve(
                configuration, CreateRecord(("ProbeA", SerialA)));

            Assert.AreEqual(string.Empty, GainCalibrationFileName(headstage.NeuropixelsV2B));
        }

        [TestMethod]
        public void HeadstageWithoutProbesIsUnchanged()
        {
            var configuration = CreateConfiguration("Headstage64Configuration");

            var headstage = Resolve(configuration, CreateRecord());

            Assert.AreSame(configuration.Headstage, headstage);
        }

        [TestMethod]
        public void MissingImplantEntryNamesProbeAndPath()
        {
            CreateCalibrationFile(SerialA);
            var configuration = CreateConfiguration("NeuropixelsV2eConfiguration");

            var error = Assert.Throws<InvalidOperationException>(
                () => Resolve(configuration, CreateRecord(("ProbeA", SerialA))));

            StringAssert.Contains(error.Message, "ProbeB");
            StringAssert.Contains(error.Message, "no implant entry");
            StringAssert.Contains(error.Message, FormatFileName(root, "<serial>"));
        }

        [TestMethod]
        public void MissingSerialNamesProbeAndPath()
        {
            CreateCalibrationFile(SerialA);
            var configuration = CreateConfiguration("NeuropixelsV2eConfiguration");

            var error = Assert.Throws<InvalidOperationException>(
                () => Resolve(configuration, CreateRecord(("ProbeA", SerialA), ("ProbeB", ""))));

            StringAssert.Contains(error.Message, "ProbeB");
            StringAssert.Contains(error.Message, "no serial number");
            StringAssert.Contains(error.Message, FormatFileName(root, "<serial>"));
        }

        [TestMethod]
        public void MissingFileNamesProbeSerialAndPath()
        {
            CreateCalibrationFile(SerialA);
            var configuration = CreateConfiguration("NeuropixelsV2eConfiguration");

            var error = Assert.Throws<FileNotFoundException>(
                () => Resolve(configuration, CreateRecord(("ProbeA", SerialA), ("ProbeB", SerialB))));

            var expected = Path.Combine(root, SerialB, SerialB + "_gainCalValues.csv");
            StringAssert.Contains(error.Message, "ProbeB");
            StringAssert.Contains(error.Message, SerialB);
            StringAssert.Contains(error.Message, expected);
            Assert.AreEqual(expected, error.FileName);
        }

        [TestMethod]
        public void EmptyCalibrationRootNamesProbeSerialAndPath()
        {
            var configuration = CreateConfiguration("NeuropixelsV2eConfiguration", enableProbeB: false);
            configuration.CalibrationRoot = string.Empty;

            var error = Assert.Throws<InvalidOperationException>(
                () => Resolve(configuration, CreateRecord(("ProbeA", SerialA))));

            StringAssert.Contains(error.Message, "ProbeA");
            StringAssert.Contains(error.Message, SerialA);
            StringAssert.Contains(error.Message, "calibrationRoot is empty");
            StringAssert.Contains(error.Message, FormatFileName("<calibrationRoot>", SerialA));
        }

        [TestMethod]
        public void RecordForAnotherSubjectIsRejected()
        {
            var configuration = CreateConfiguration("Headstage64Configuration");
            var record = CreateRecord();
            record.Subject = "BA-99999";

            var error = Assert.Throws<InvalidOperationException>(() => Resolve(configuration, record));

            StringAssert.Contains(error.Message, "BA-99999");
            StringAssert.Contains(error.Message, "BA-12345");
        }

        [TestMethod]
        public void CopyConfigurationFilesCopiesResolvedFiles()
        {
            // The calibration file comes from the record and the probe interface file from the
            // configuration. A file shared by both probes is copied once.
            CreateCalibrationFile(SerialA);
            CreateCalibrationFile(SerialB);
            var probeInterface = Path.Combine(root, "channels.json");
            File.WriteAllText(probeInterface, "{}");
            var configuration = CreateConfiguration("NeuropixelsV2eConfiguration");
            var headstage = (NeuropixelsV2eConfiguration)configuration.Headstage;
            headstage.ProbeA.ProbeConfiguration.ProbeInterfaceFileName = probeInterface;
            headstage.ProbeB.ProbeConfiguration.ProbeInterfaceFileName = probeInterface;
            var destination = Path.Combine(root, "dataset");
            var copier = new CopyConfigurationFiles { Path = destination };

            var copied = copier
                .Process(Observable.Return(Tuple.Create(configuration, CreateRecord(("ProbeA", SerialA), ("ProbeB", SerialB)))))
                .ToArray()
                .Wait();

            CollectionAssert.AreEquivalent(
                new[]
                {
                    Path.Combine(destination, SerialA + "_gainCalValues.csv"),
                    Path.Combine(destination, SerialB + "_gainCalValues.csv"),
                    Path.Combine(destination, "channels.json")
                },
                copied);
            Assert.IsTrue(copied.All(File.Exists));
        }

        [TestMethod]
        public void CopyConfigurationFilesRejectsDistinctFilesSharingName()
        {
            CreateCalibrationFile(SerialA);
            var first = Path.Combine(root, "a", "channels.json");
            var second = Path.Combine(root, "b", "channels.json");
            Directory.CreateDirectory(Path.GetDirectoryName(first)!);
            Directory.CreateDirectory(Path.GetDirectoryName(second)!);
            File.WriteAllText(first, "{}");
            File.WriteAllText(second, "{}");
            CreateCalibrationFile(SerialB);
            var configuration = CreateConfiguration("NeuropixelsV2eConfiguration");
            var headstage = (NeuropixelsV2eConfiguration)configuration.Headstage;
            headstage.ProbeA.ProbeConfiguration.ProbeInterfaceFileName = first;
            headstage.ProbeB.ProbeConfiguration.ProbeInterfaceFileName = second;
            var copier = new CopyConfigurationFiles { Path = Path.Combine(root, "dataset") };

            var error = Assert.Throws<InvalidOperationException>(() => copier
                .Process(Observable.Return(Tuple.Create(configuration, CreateRecord(("ProbeA", SerialA), ("ProbeB", SerialB)))))
                .ToArray()
                .Wait());

            StringAssert.Contains(error.Message, "channels.json");
        }
    }
}
