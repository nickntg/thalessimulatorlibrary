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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThalesSim.Core.Resources;
using ThalesSim.Core.Utility;
using log4net;

namespace ThalesSim.Core.Message
{
    /// <summary>
    /// The parser does the job of figuring out what a request message says.
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Parses a request message to key/value pairs,
        /// according to field definitions.
        /// </summary>
        /// <param name="msg">Message to parse.</param>
        /// <param name="fields">Field definitions.</param>
        /// <param name="pairs">Key/value pairs parsed.</param>
        /// <returns></returns>
        public static string Parse (StreamMessage msg, Fields fields, MessageKeyValuePairs pairs)
        {
            var log = LogManager.GetLogger("Parser");

            // Don't skip anything at first.
            foreach (var fld in fields.MessageFields)
            {
                fld.Skip = false;
            }

            var index = 0;
            while (index <= fields.MessageFields.Count - 1)
            {
                var fld = fields.MessageFields[index];

                var repetitions = 1;
                if (!string.IsNullOrEmpty(fld.Repetitions))
                {
                    repetitions = Convert.ToInt32(fld.Repetitions.IsNumeric() ? fld.Repetitions : pairs.Item(fld.Repetitions));
                }

                // Do we have static repetitions.
                if (fld.StaticRepetitions)
                {
                    /*
                     * Yes. 
                     * This is what we must do for static repetitions:
                     *
                     * 1. Scan ahead and see if subsequent fields also require static repetitions.
                     *    If so, we'll treat all fields as a group.
                     * 2. Save the aforementioned field group and remove them from the fields.
                     * 3. Insert the aforementioned field group in the fields for the number
                     *    of repetitions. Ensure that the dynamically inserted fields do not
                     *    require any number of repetitions or static repetitions.
                     * 4. Let the parsing continue as usual.
                     */

                    // Look ahead.
                    var nextNonStaticRepField = index + 1;
                    while (nextNonStaticRepField <= fields.MessageFields.Count -1 && fields.MessageFields[nextNonStaticRepField].StaticRepetitions)
                    {
                        nextNonStaticRepField++;
                    }

                    // Save fields.
                    var dynamicFields = new List<Field>();

                    for (var i = index; i <= nextNonStaticRepField - 1; i++)
                    {
                        dynamicFields.Add(fields.MessageFields[i]);
                    }

                    // Remove from the original fields list.
                    for (var i = index; i <= nextNonStaticRepField - 1; i++)
                    {
                        fields.MessageFields.RemoveAt(index);
                    }

                    // Insert for all repetitions.
                    var insertPos = index;
                    var fieldList = new List<string>();
                    for (var i = 1; i <= repetitions; i++)
                    {
                        for (var j = 0; j <= dynamicFields.Count - 1; j++)
                        {
                            var newFld = dynamicFields[j].Clone();
                            newFld.Repetitions = string.Empty;
                            newFld.StaticRepetitions = false;

                            // Save the ORIGINAL field name.
                            if (!fieldList.Contains(newFld.Name))
                            {
                                fieldList.Add(newFld.Name);
                            }

                            // Alter the field name to signify the repetition number.
                            newFld.Name = newFld.Name + " #" + i.ToString();

                            // If a field is dependent upon a field that is repeated,
                            // make sure to correct the dependency.
                            if (fieldList.Contains(newFld.DependentField))
                            {
                                newFld.DependentField = newFld.DependentField + " #" + i.ToString();
                            }

                            // Do the same as above for dynamic length.
                            if (fieldList.Contains(newFld.DynamicLength))
                            {
                                newFld.DynamicLength = newFld.DynamicLength + " #" + i.ToString();
                            }

                            // Add the field.
                            fields.MessageFields.Insert(insertPos, newFld);
                            insertPos++;
                        }
                    }

                    // We've dynamically inserted the required fields, hence no repetition is required.
                    repetitions = 1;

                    // Update the field because it has changed.
                    fld = fields.MessageFields[index];
                }

                for (var j = 1; j <= repetitions; j++)
                {
                    /*
                     * Criteria to process field.
                     * 1. If we should not skip this field...
                     * 2. If there is a dependent field which has already been processed
                     * 3. If the value of the dependent field matches what we expect...
                     * 4. If there is no dependent field...
                     * 5. If the dependent field has not been processed and we don *t have 
                     *    a dependent value...
                     *
                     * ((1) AND (2) AND (3)) OR (4) OR (5).
                     *
                     * (1) ==> Several fields may depend upon one dependent field. When one of
                     *         these fields is parse, we don *t want to parse the others. Therefore,
                     *         when one field with a dependent field is parsed, we mark all other
                     *         fields that depend on the dependent field with the Skip=True flag.
                     * (2) ==> If this field depends on another, we want to try and parse it only
                     *         if the dependent field has been already parsed.
                     * (3) ==> If this field depends on another field *s presence and value, we
                     *         want to parse the field only if the above conditions are met.
                     * (4) ==> Self-explanatory.
                     * (5) ==> The current field depends on the presence of another field which has
                     *         not been parsed, but not its value.
                     */
                    if (((!fld.Skip) && 
                        (!string.IsNullOrEmpty(fld.DependentField) && pairs.ContainsKey(fld.DependentField)) &&
                        (fld.DependentValues.Count == 0 || fld.DependentValues.Contains(pairs.Item(fld.DependentField)))) ||
                       (string.IsNullOrEmpty(fld.DependentField)) ||
                       (!string.IsNullOrEmpty(fld.DependentField) && !pairs.ContainsKey(fld.DependentField) && fld.DependentValues.Count == 0))
                    {
                        string val;

                        if (fld.SkipUntil)
                        {
                            try
                            {
                                // If we're supposed to skip until we encounter
                                // a valid value, keep on reading.
                                do
                                {
                                    val = msg.Substring(fld.Length);
                                    if (fld.ValidValues.Contains(val))
                                    {
                                        break;
                                    }
                                    msg.Index++;
                                } while (fld.ValidValues.Contains(val));
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                if (fld.AllowNotFoundValid)
                                {
                                    val = string.Empty;
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty(fld.ParseUntil))
                        {
                            // Parse until a specific value is found.
                            // Note that we're looking for a single
                            // character value only.
                            var tempval = string.Empty;
                            do
                            {
                                val = msg.Substring(1);
                                if (fld.ParseUntil == val)
                                {
                                    msg.Index--;
                                    break;
                                }

                                tempval += val;
                                msg.Index++;
                            } while (true);
                            val = tempval;
                        }
                        else
                        {
                            // Else, read the current field.
                            // Get the dynamic length, if appropriate.
                            if (!string.IsNullOrEmpty(fld.DynamicLength))
                            {
                                // Find out if the dynamic field is
                                // numeric or otherwise and perform
                                // the necessary conversion.
                                foreach (var scannedFld in fields.MessageFields.Where(scannedFld => scannedFld.Name == fld.DependentField))
                                {
                                    fld.Length = scannedFld.Type == FieldType.Hexadecimal ? Convert.ToInt32(pairs.Item(fld.DynamicLength), 16) : Convert.ToInt32(pairs.Item(fld.DynamicLength));
                                    break;
                                }
                            }

                            if (fld.Length != 0)
                            {
                                // Normal & binary reads.
                                val = fld.Type != FieldType.Binary ? msg.Substring(fld.Length) : msg.Substring(fld.Length*2);
                            }
                            else
                            {
                                // Else read the rest of the message.
                                val = msg.Substring(msg.CharsLeft);
                            }
                        }

                        if (fld.OptionValues.Count == 0 || fld.OptionValues.Contains(val))
                        {
                            try
                            {
                                // Check valid values.
                                if (fld.ValidValues.Count > 0 && !fld.ValidValues.Contains(val))
                                {
                                    log.ErrorFormat("Invalid value detected for field [{0}].", fld.Name);
                                    log.ErrorFormat("Received [{0}] but can be one of [{1}]", val,
                                                    GetCommaSeparatedListWithValue(fld.ValidValues));
                                    throw new InvalidOperationException(
                                        string.Format("Invalid value [{0}] for field [{1}]", val, fld.Name));
                                }

                                // Check format.
                                switch (fld.Type)
                                {
                                    case FieldType.Hexadecimal:
                                    case FieldType.Binary:
                                        if (!val.IsHex())
                                        {
                                            log.ErrorFormat("Invalid value detected for field [{0}]", fld.Name);
                                            log.ErrorFormat("Received [{0}] but expected a hexadecimal value", val);
                                            throw new InvalidOperationException(
                                                string.Format("Invalid value [{0}] for field [{1}].", val, fld.Name));
                                        }
                                        break;
                                    case FieldType.Numeric:
                                        if (!val.IsNumeric())
                                        {
                                            log.ErrorFormat("Invalid value detected for field [{0}]", fld.Name);
                                            log.ErrorFormat("Received [{0}] but expected a numeric value", val);
                                            throw new InvalidOperationException(
                                                string.Format("Invalid value [{0}] for field [{1}].", val, fld.Name));
                                        }
                                        break;
                                }
                            }
                            catch (Exception)
                            {
                                // If a rejection code is specified, use it.
                                if (!string.IsNullOrEmpty(fld.RejectionCode))
                                {
                                    return fld.RejectionCode;
                                }

                                throw;
                            }

                            // Add the value.
                            if (repetitions == 1)
                            {
                                pairs.Add(fld.Name, val);
                            }
                            else
                            {
                                pairs.Add(fld.Name + " #" + j.ToString(), val);
                            }

                            // Advance index.
                            msg.Index += fld.Type != FieldType.Binary ? fld.Length : fld.Length*2;

                            // Field parsed if repetitions are done.
                            fld.Skip = (j == repetitions);

                            // If there were a dependent field, then mark all other
                            // fields that had the same dependency so we won't try
                            // to parse them.
                            if (!string.IsNullOrEmpty(fld.DependentField))
                            {
                                for (var z = index + 1; z < fields.MessageFields.Count; z++)
                                {
                                    if (fields.MessageFields[z].DependentField == fld.DependentField &&
                                        fields.MessageFields[z].ExclusiveDependency)
                                    {
                                        fields.MessageFields[z].Skip = true;
                                    }
                                }
                            }
                        }
                    }

                    if (msg.CharsLeft == 0)
                    {
                        break;
                    }
                }

                if (msg.CharsLeft == 0)
                {
                    break;
                }

                index++;
            }

            return ErrorCodes.ER_00_NO_ERROR;
        }

        private static string GetCommaSeparatedListWithValue (List<string> lst)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < lst.Count; i++)
            {
                if (i < lst.Count)
                {
                    sb.Append(lst[i] + ",");
                }
                else
                {
                    sb.Append(lst[i]);
                }
            }
            return sb.ToString();
        }
    }
}
