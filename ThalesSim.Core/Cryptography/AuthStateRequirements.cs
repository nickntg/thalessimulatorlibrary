/*
 This program is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program; if not, write to the Free Software
 Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System.Collections.Generic;
using System.Linq;
using ThalesSim.Core.Cryptography.LMK;

namespace ThalesSim.Core.Cryptography
{
    /// <summary>
    /// Contains all authorized state requirements.
    /// </summary>
    public class AuthStateRequirements
    {
        private static readonly List<AuthStateRequirement> Reqs = new List<AuthStateRequirement> 
                {new AuthStateRequirement {Function = KeyFunction.Generate, Pair = LmkPair.Pair04_05, Variant = 0, Requirement = StateRequirementType.NeedsAuthorizedState}, 
                 new AuthStateRequirement{Function = KeyFunction.Generate, Pair = LmkPair.Pair06_07, Variant = 0, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Generate, Pair = LmkPair.Pair14_15, Variant = 0, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Generate, Pair = LmkPair.Pair16_17, Variant = 0, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Generate, Pair = LmkPair.Pair22_23, Variant = 0, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Generate, Pair = LmkPair.Pair26_27, Variant = 0, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Generate, Pair = LmkPair.Pair28_29, Variant = 0, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Generate, Pair = LmkPair.Pair30_31, Variant = 0, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Generate, Pair = LmkPair.Pair04_05, Variant = 1, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Generate, Pair = LmkPair.Pair28_29, Variant = 1, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Generate, Pair = LmkPair.Pair04_05, Variant = 2, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Generate, Pair = LmkPair.Pair28_29, Variant = 2, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Generate, Pair = LmkPair.Pair28_29, Variant = 3, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Generate, Pair = LmkPair.Pair14_15, Variant = 4, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Generate, Pair = LmkPair.Pair28_29, Variant = 4, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Generate, Pair = LmkPair.Pair28_29, Variant = 5, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Import, Pair = LmkPair.Pair06_07, Variant = 0, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Import, Pair = LmkPair.Pair14_15, Variant = 0, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Import, Pair = LmkPair.Pair16_17, Variant = 0, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Import, Pair = LmkPair.Pair22_23, Variant = 0, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Import, Pair = LmkPair.Pair26_27, Variant = 0, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Import, Pair = LmkPair.Pair28_29, Variant = 0, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Import, Pair = LmkPair.Pair30_31, Variant = 0, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Import, Pair = LmkPair.Pair04_05, Variant = 1, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Import, Pair = LmkPair.Pair28_29, Variant = 1, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Import, Pair = LmkPair.Pair04_05, Variant = 2, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Import, Pair = LmkPair.Pair28_29, Variant = 2, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Import, Pair = LmkPair.Pair28_29, Variant = 3, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Import, Pair = LmkPair.Pair14_15, Variant = 4, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Import, Pair = LmkPair.Pair28_29, Variant = 4, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Import, Pair = LmkPair.Pair28_29, Variant = 5, Requirement = StateRequirementType.DoesNotNeedAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Export, Pair = LmkPair.Pair06_07, Variant = 0, Requirement = StateRequirementType.NeedsAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Export, Pair = LmkPair.Pair14_15, Variant = 0, Requirement = StateRequirementType.NeedsAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Export, Pair = LmkPair.Pair16_17, Variant = 0, Requirement = StateRequirementType.NeedsAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Export, Pair = LmkPair.Pair22_23, Variant = 0, Requirement = StateRequirementType.NeedsAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Export, Pair = LmkPair.Pair26_27, Variant = 0, Requirement = StateRequirementType.NeedsAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Export, Pair = LmkPair.Pair28_29, Variant = 0, Requirement = StateRequirementType.NeedsAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Export, Pair = LmkPair.Pair30_31, Variant = 0, Requirement = StateRequirementType.NeedsAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Export, Pair = LmkPair.Pair04_05, Variant = 1, Requirement = StateRequirementType.NeedsAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Export, Pair = LmkPair.Pair28_29, Variant = 1, Requirement = StateRequirementType.NeedsAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Export, Pair = LmkPair.Pair04_05, Variant = 2, Requirement = StateRequirementType.NeedsAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Export, Pair = LmkPair.Pair28_29, Variant = 2, Requirement = StateRequirementType.NeedsAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Export, Pair = LmkPair.Pair28_29, Variant = 3, Requirement = StateRequirementType.NeedsAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Export, Pair = LmkPair.Pair14_15, Variant = 4, Requirement = StateRequirementType.NeedsAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Export, Pair = LmkPair.Pair28_29, Variant = 4, Requirement = StateRequirementType.NeedsAuthorizedState},
                 new AuthStateRequirement{Function = KeyFunction.Export, Pair = LmkPair.Pair28_29, Variant = 5, Requirement = StateRequirementType.NeedsAuthorizedState}};

        /// <summary>
        /// Returns an authorized state requirement.
        /// </summary>
        /// <param name="function">Function requested.</param>
        /// <param name="pair">LMK pair for which function is requested.</param>
        /// <param name="variant">Variant requested.</param>
        /// <returns>Requirement.</returns>
        public static StateRequirementType GetRequirement (KeyFunction function, LmkPair pair, int variant)
        {
            foreach (var req in Reqs.Where(req => req.Function == function && req.Pair == pair && req.Variant == variant))
            {
                return req.Requirement;
            }

            return StateRequirementType.NotAllowed;
        }
    }
}
