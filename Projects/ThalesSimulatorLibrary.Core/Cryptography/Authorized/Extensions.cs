using System.Collections.Generic;
using System.Linq;
using ThalesSimulatorLibrary.Core.Cryptography.LMK;

namespace ThalesSimulatorLibrary.Core.Cryptography.Authorized
{
    public static class Extensions
    {
        private static readonly List<StateRequirement> Requirements = new()
        {
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair0405, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair0607, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair1415, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair1617, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair2223, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair2627, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair2829, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair3031, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair0405, Variant = "1",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair2829, Variant = "1",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair0405, Variant = "2",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair2829, Variant = "2",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair2829, Variant = "3",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair1415, Variant = "4",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair2829, Variant = "4",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair2829, Variant = "5",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Import, LmkPair = LmkPair.Pair0607, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Import, LmkPair = LmkPair.Pair1415, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Import, LmkPair = LmkPair.Pair1617, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Import, LmkPair = LmkPair.Pair2223, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Import, LmkPair = LmkPair.Pair2627, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Import, LmkPair = LmkPair.Pair2829, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Import, LmkPair = LmkPair.Pair3031, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Import, LmkPair = LmkPair.Pair0405, Variant = "1",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Import, LmkPair = LmkPair.Pair2829, Variant = "1",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Import, LmkPair = LmkPair.Pair0405, Variant = "2",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Import, LmkPair = LmkPair.Pair2829, Variant = "2",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Import, LmkPair = LmkPair.Pair2829, Variant = "3",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Import, LmkPair = LmkPair.Pair1415, Variant = "4",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Import, LmkPair = LmkPair.Pair2829, Variant = "4",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Import, LmkPair = LmkPair.Pair2829, Variant = "5",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Export, LmkPair = LmkPair.Pair0607, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Export, LmkPair = LmkPair.Pair1415, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Export, LmkPair = LmkPair.Pair1617, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Export, LmkPair = LmkPair.Pair2223, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Export, LmkPair = LmkPair.Pair2627, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Export, LmkPair = LmkPair.Pair2829, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Export, LmkPair = LmkPair.Pair3031, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Export, LmkPair = LmkPair.Pair0405, Variant = "1",
                Requirement = AuthorizedStateRequirement.AuthorizedStateRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Export, LmkPair = LmkPair.Pair2829, Variant = "1",
                Requirement = AuthorizedStateRequirement.AuthorizedStateRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Export, LmkPair = LmkPair.Pair0405, Variant = "2",
                Requirement = AuthorizedStateRequirement.AuthorizedStateRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Export, LmkPair = LmkPair.Pair2829, Variant = "2",
                Requirement = AuthorizedStateRequirement.AuthorizedStateRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Export, LmkPair = LmkPair.Pair2829, Variant = "3",
                Requirement = AuthorizedStateRequirement.AuthorizedStateRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Export, LmkPair = LmkPair.Pair1415, Variant = "4",
                Requirement = AuthorizedStateRequirement.AuthorizedStateRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Export, LmkPair = LmkPair.Pair2829, Variant = "4",
                Requirement = AuthorizedStateRequirement.AuthorizedStateRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Export, LmkPair = LmkPair.Pair2829, Variant = "5",
                Requirement = AuthorizedStateRequirement.AuthorizedStateRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair2829, Variant = "7",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Export, LmkPair = LmkPair.Pair2829, Variant = "7",
                Requirement = AuthorizedStateRequirement.AuthorizedStateRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Import, LmkPair = LmkPair.Pair2829, Variant = "7",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair3233, Variant = "0",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Generate, LmkPair = LmkPair.Pair3435, Variant = "1",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Export, LmkPair = LmkPair.Pair3435, Variant = "1",
                Requirement = AuthorizedStateRequirement.AuthorizedStateRequired
            },
            new StateRequirement
            {
                Function = KeyFunction.Import, LmkPair = LmkPair.Pair3435, Variant = "1",
                Requirement = AuthorizedStateRequirement.AuthorizedStateNotRequired
            },
        };
        public static AuthorizedStateRequirement GetAuthorizedStateRequirementForKeyFunction(this KeyFunction function,
            string variant, LmkPair lmkPair)
        {
            var requirement =
                Requirements.FirstOrDefault(x => x.Function == function && x.LmkPair == lmkPair && x.Variant == variant);

            return requirement?.Requirement ?? AuthorizedStateRequirement.NotAllowed;
        }
    }

    internal class StateRequirement
    {
        public KeyFunction Function { get; set; }
        public LmkPair LmkPair { get; set; }
        public string Variant { get; set; }
        public AuthorizedStateRequirement Requirement { get; set; }
    }
}
