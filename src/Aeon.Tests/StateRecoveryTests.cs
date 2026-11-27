using Aeon.Acquisition;
using Aeon.Environment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Aeon.Tests
{
    [TestClass]
    public class StateRecoveryTests
    {
        static string GetRecoveryFileName<TState>(string name) => $"~{name}.{typeof(TState).Name}.tmp";

        [TestMethod]
        public void EnvironmentSubjectState_RepeatedRestarts_RecoversActiveSubjects()
        {
            const string SubjectId = "subject";
            var name = nameof(EnvironmentSubjectState_RepeatedRestarts_RecoversActiveSubjects);
            var fileName = GetRecoveryFileName<SubjectStateRecovery>(name);
            try
            {
                var state = new SubjectStateRecovery();
                state.ActiveSubjects.Add(new EnvironmentSubjectStateEntry
                {
                    Id = SubjectId,
                    Weight = 20,
                    ReferenceWeight = 25
                });
                StateRecovery<SubjectStateRecovery>.Serialize(name, state);

                var source = new EnvironmentSubjectState { Name = name };
                for (int restart = 1; restart <= 3; restart++)
                {
                    var recovered = new List<EnvironmentSubjectStateMetadata>();
                    using (source.Process().Subscribe(recovered.Add)) { }
                    Assert.AreEqual(1, recovered.Count, $"restart {restart}");
                    Assert.AreEqual(EnvironmentSubjectChangeType.Remain, recovered[0].Type, $"restart {restart}");
                    Assert.AreEqual(SubjectId, recovered[0].Id, $"restart {restart}");
                    Assert.IsTrue(File.Exists(fileName), $"restart {restart}");
                }
            }
            finally
            {
                if (File.Exists(fileName)) File.Delete(fileName);
            }
        }

        [TestMethod]
        public void EnvironmentSubjectState_NoActiveSubjects_WritesNoFile()
        {
            var name = nameof(EnvironmentSubjectState_NoActiveSubjects_WritesNoFile);
            var fileName = GetRecoveryFileName<SubjectStateRecovery>(name);
            try
            {
                var source = new EnvironmentSubjectState { Name = name };
                var recovered = new List<EnvironmentSubjectStateMetadata>();
                using (source.Process().Subscribe(recovered.Add)) { }
                Assert.AreEqual(0, recovered.Count);
                Assert.IsFalse(File.Exists(fileName));
            }
            finally
            {
                if (File.Exists(fileName)) File.Delete(fileName);
            }
        }

        [TestMethod]
        public void EnvironmentState_RepeatedRestarts_RecoversState()
        {
            const EnvironmentStateType ExpectedType = EnvironmentStateType.Experiment;
            var name = nameof(EnvironmentState_RepeatedRestarts_RecoversState);
            var fileName = GetRecoveryFileName<EnvironmentStateRecovery>(name);
            try
            {
                StateRecovery<EnvironmentStateRecovery>.Serialize(name, new EnvironmentStateRecovery
                {
                    Type = ExpectedType
                });

                var source = new EnvironmentState { Name = name };
                for (int restart = 1; restart <= 3; restart++)
                {
                    var recovered = new List<EnvironmentStateMetadata>();
                    using (source.Process().Subscribe(recovered.Add)) { }
                    Assert.AreEqual(1, recovered.Count, $"restart {restart}");
                    Assert.AreEqual(ExpectedType, recovered[0].Type, $"restart {restart}");
                    Assert.IsTrue(File.Exists(fileName), $"restart {restart}");
                }
            }
            finally
            {
                if (File.Exists(fileName)) File.Delete(fileName);
            }
        }
    }
}
