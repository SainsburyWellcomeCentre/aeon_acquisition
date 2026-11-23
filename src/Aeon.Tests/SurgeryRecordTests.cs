using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Aeon.Ephys;
using Bonsai.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Aeon.Tests
{
    [TestClass]
    public class SurgeryRecordTests
    {
        const string RecordYaml =
            "subject: BA-12345\n" +
            "implants:\n" +
            "  - probe: ProbeA\n" +
            "    serialNumber: \"12345678\"\n" +
            "    targetArea: CA1\n" +
            "    insertionNumber: 1\n" +
            "    implantationDate: \"2026-08-20\"\n" +
            "    probePartNumber: NP2014\n" +
            "  - probe: ProbeB\n" +
            "    serialNumber: \"23456789\"\n" +
            "    targetArea: mPFC\n" +
            "    insertionNumber: 2\n" +
            "    implantationDate: \"2026-08-20\"\n" +
            "    probePartNumber: NP2014\n";

        internal static T Deserialize<T>(string yaml)
        {
            // Builds the deserializer as a workflow does. The test exercises the expression the
            // operator emits rather than a method it happens to call.
            var builder = new DeserializeFromYaml { Type = new TypeMapping<T>() };
            var source = Expression.Constant(Observable.Return(yaml), typeof(IObservable<string>));
            var expression = builder.Build(new[] { source });
            return Expression.Lambda<Func<IObservable<T>>>(expression)
                .Compile()
                .Invoke()
                .Wait();
        }

        static void AssertExampleRecord(ImplantedSubject record)
        {
            Assert.AreEqual("BA-12345", record.Subject);
            Assert.AreEqual(2, record.Implants.Count);
            var probeA = record.Implants[0];
            Assert.AreEqual("ProbeA", probeA.Probe);
            Assert.AreEqual("12345678", probeA.SerialNumber);
            Assert.AreEqual("CA1", probeA.TargetArea);
            Assert.AreEqual(1, probeA.InsertionNumber);
            Assert.AreEqual("2026-08-20", probeA.ImplantationDate);
            Assert.AreEqual("NP2014", probeA.ProbePartNumber);
            Assert.AreEqual("ProbeB", record.Implants[1].Probe);
            Assert.AreEqual("23456789", record.Implants[1].SerialNumber);
        }

        [TestMethod]
        public void RecordYamlRoundTrips()
        {
            // The record files already on the share deserialise. The package serialiser writes
            // them back in a form that deserialises to the same record.
            var record = Deserialize<ImplantedSubject>(RecordYaml);
            AssertExampleRecord(record);

            var yaml = new SerializeToYaml().Process(Observable.Return(record)).Wait();
            AssertExampleRecord(Deserialize<ImplantedSubject>(yaml));
        }

        [TestMethod]
        public void RecordSerialisesToJsonWithCamelCaseKeys()
        {
            // The record is written into the session metadata. Its keys are the contract the
            // data readers depend on.
            var record = Deserialize<ImplantedSubject>(RecordYaml);
            var json = JObject.Parse(new SerializeToJson().Process(Observable.Return(record)).Wait());

            CollectionAssert.AreEqual(
                new[] { "subject", "implants" },
                json.Properties().Select(property => property.Name).ToArray());
            var implant = (JObject)json["implants"]![0]!;
            CollectionAssert.AreEqual(
                new[]
                {
                    "probe", "serialNumber", "targetArea", "insertionNumber",
                    "implantationDate", "probePartNumber"
                },
                implant.Properties().Select(property => property.Name).ToArray());
            Assert.AreEqual("12345678", (string?)implant["serialNumber"]);
            Assert.AreEqual(1, (int)implant["insertionNumber"]!);
        }

        [TestMethod]
        [DataRow("NeuropixelsV2eConfiguration")]
        [DataRow("NeuropixelsV2eBetaConfiguration")]
        [DataRow("Headstage64Configuration")]
        public void YamlCarriesCalibrationRoot(string headstageType)
        {
            var yaml = string.Join("\n",
                "subject: BA-12345",
                "calibrationRoot: D:/probe_settings",
                "headstage:",
                "  type: " + headstageType);

            var configuration = Deserialize<EphysConfiguration>(yaml);

            Assert.AreEqual("D:/probe_settings", configuration.CalibrationRoot);
            Assert.AreEqual(headstageType, configuration.Headstage.GetType().Name);
        }

        [TestMethod]
        public void YamlNamingGainCalibrationFileIsRejected()
        {
            // The gain calibration file comes from the surgery record. A configuration file
            // still naming one fails to load rather than being silently ignored.
            var yaml = string.Join("\n",
                "subject: BA-12345",
                "headstage:",
                "  type: NeuropixelsV2eConfiguration",
                "  probeA:",
                "    probeConfiguration:",
                "      gainCalibrationFileName: gain.csv");

            Assert.Throws<YamlDotNet.Core.YamlException>(
                () => Deserialize<EphysConfiguration>(yaml));
        }

        static EphysConfiguration CreateConfiguration(Headstage headstage)
        {
            return new EphysConfiguration { Subject = "BA-12345", Headstage = headstage };
        }

        static string ReadRecord(string folder, EphysConfiguration configuration)
        {
            var reader = new ReadSubjectRecord { Path = folder };
            return reader.Process(Observable.Return(configuration)).Wait();
        }

        static void AssertEmptyRecord(string text)
        {
            var record = Deserialize<ImplantedSubject>(text);
            Assert.AreEqual("BA-12345", record.Subject);
            Assert.AreEqual(0, record.Implants.Count);
        }

        [TestMethod]
        public void ReadSubjectRecordReadsRecordNamedBySubject()
        {
            var folder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(folder);
            try
            {
                File.WriteAllText(Path.Combine(folder, "BA-12345.yaml"), RecordYaml);

                var text = ReadRecord(folder, CreateConfiguration(new NeuropixelsV2eConfiguration()));

                AssertExampleRecord(Deserialize<ImplantedSubject>(text));
            }
            finally
            {
                Directory.Delete(folder, true);
            }
        }

        [TestMethod]
        public void ReadSubjectRecordReadsExistingRecordWithoutProbes()
        {
            // An implanted subject keeps its record in the metadata even when no probe is used.
            var folder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(folder);
            try
            {
                File.WriteAllText(Path.Combine(folder, "BA-12345.yaml"), RecordYaml);

                var text = ReadRecord(folder, CreateConfiguration(new Headstage64Configuration()));

                AssertExampleRecord(Deserialize<ImplantedSubject>(text));
            }
            finally
            {
                Directory.Delete(folder, true);
            }
        }

        [TestMethod]
        public void ReadSubjectRecordNamesMissingFileWhenProbeEnabled()
        {
            // An enabled probe needs the record to find its gain calibration file.
            var folder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var configuration = CreateConfiguration(new NeuropixelsV2eConfiguration());

            var error = Assert.Throws<FileNotFoundException>(() => ReadRecord(folder, configuration));

            StringAssert.Contains(error.Message, "BA-12345");
            StringAssert.Contains(error.Message, Path.Combine(folder, "BA-12345.yaml"));
        }

        [TestMethod]
        public void MissingRecordIsEmptyForHeadstageWithoutProbes()
        {
            // A 64-channel headstage has no probe to calibrate. It needs no surgery record.
            var folder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            var text = ReadRecord(folder, CreateConfiguration(new Headstage64Configuration()));

            AssertEmptyRecord(text);
        }

        [TestMethod]
        public void MissingRecordIsEmptyWhenAllProbesDisabled()
        {
            // A Neuropixels headstage used only for its BNO055 needs no surgery record.
            var folder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var headstage = new NeuropixelsV2eBetaConfiguration();
            headstage.ProbeA.Enable = false;
            headstage.ProbeB.Enable = false;

            var text = ReadRecord(folder, CreateConfiguration(headstage));

            AssertEmptyRecord(text);
        }
    }
}
