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
    public class Parser
    {
        public static string Parse (StreamMessage msg, Fields fields, MessageKeyValuePairs pairs)
        {
            var log = LogManager.GetLogger("Parser");

            foreach (var fld in fields.MessageFields)
            {
                fld.Skip = true;
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

                if (fld.StaticRepetitions)
                {
                    var nextNonStaticRepField = index + 1;
                    while (nextNonStaticRepField <= fields.MessageFields.Count -1 && fields.MessageFields[nextNonStaticRepField].StaticRepetitions)
                    {
                        nextNonStaticRepField++;
                    }

                    var dynamicFields = new List<Field>();

                    for (var i = index; i <= nextNonStaticRepField - 1; i++)
                    {
                        dynamicFields.Add(fields.MessageFields[i]);
                    }

                    for (var i = index; i <= nextNonStaticRepField - 1; i++)
                    {
                        fields.MessageFields.RemoveAt(index);
                    }

                    var insertPos = index;
                    var fieldList = new List<string>();
                    for (var i = 1; i <= repetitions; i++)
                    {
                        for (var j = 0; j <= dynamicFields.Count - 1; j++)
                        {
                            var newFld = dynamicFields[j].Clone();
                            newFld.Repetitions = string.Empty;
                            newFld.StaticRepetitions = false;

                            if (!fieldList.Contains(newFld.Name))
                            {
                                fieldList.Add(newFld.Name);
                            }

                            newFld.Name = newFld.Name + " #" + i.ToString();

                            if (fieldList.Contains(newFld.DependentField))
                            {
                                newFld.DependentField = newFld.DependentField + " #" + i.ToString();
                            }

                            if (fieldList.Contains(newFld.DynamicLength))
                            {
                                newFld.DynamicLength = newFld.DynamicLength + " #" + i.ToString();
                            }

                            fields.MessageFields.Insert(insertPos, newFld);
                            insertPos++;
                        }
                    }

                    repetitions = 1;

                    fld = fields.MessageFields[index];
                }

                for (var j = 1; j <= repetitions; j++)
                {
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
                            if (!string.IsNullOrEmpty(fld.DynamicLength))
                            {
                                foreach (var scannedFld in fields.MessageFields.Where(scannedFld => scannedFld.Name == fld.DependentField))
                                {
                                    fld.Length = scannedFld.Type == FieldType.Hexadecimal ? Convert.ToInt32(pairs.Item(fld.DynamicLength), 16) : Convert.ToInt32(pairs.Item(fld.DynamicLength));
                                    break;
                                }
                            }

                            if (fld.Length != 0)
                            {
                                val = fld.Type != FieldType.Binary ? msg.Substring(fld.Length) : msg.Substring(fld.Length*2);
                            }
                            else
                            {
                                val = msg.Substring(msg.CharsLeft);
                            }
                        }

                        if (fld.OptionValues.Count == 0 || fld.OptionValues.Contains(val))
                        {
                            try
                            {
                                if (fld.ValidValues.Count > 0 && !fld.ValidValues.Contains(val))
                                {
                                    log.ErrorFormat("Invalid value detected for field [{0}].", fld.Name);
                                    log.ErrorFormat("Received [{0}] but can be one of [{1}]", val,
                                                    GetCommaSeparatedListWithValue(fld.ValidValues));
                                    throw new InvalidOperationException(
                                        string.Format("Invalid value [{0}] for field [{1}]", val, fld.Name));
                                }

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
                                if (!string.IsNullOrEmpty(fld.RejectionCode))
                                {
                                    return fld.RejectionCode;
                                }

                                throw;
                            }

                            if (repetitions == 1)
                            {
                                pairs.Add(fld.Name, val);
                            }
                            else
                            {
                                pairs.Add(fld.Name + " #" + j.ToString(), val);
                            }

                            msg.Index += fld.Type != FieldType.Binary ? fld.Length : fld.Length*2;

                            fld.Skip = (j == repetitions);

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
