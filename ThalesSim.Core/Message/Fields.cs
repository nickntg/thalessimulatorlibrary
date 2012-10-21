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
using System.Data;
using ThalesSim.Core.Properties;
using ThalesSim.Core.Utility;

namespace ThalesSim.Core.Message
{
    public class Fields
    {
        public List<Field> MessageFields { get; set; }

        public bool IsDynamic { get; set; }

        public Fields()
        {
            MessageFields = new List<Field>();
        }

        public Fields Clone()
        {
            var o = new Fields();
            foreach (var field in MessageFields)
            {
                o.MessageFields.Add(field.Clone());
            }

            o.IsDynamic = IsDynamic;
            return o;
        }

        public static Fields ReadXmlDefinition (string xmlFile)
        {
            return RecurseXmlDefinition(Settings.Default.HostCommandDefinitions.AppendTrailingSeparator() + xmlFile);
        }

        private static Fields RecurseXmlDefinition (string xmlFile)
        {
            var fields = new Fields();

            using (var ds = new DataSet())
            {
                ds.ReadXml(xmlFile);

                foreach (DataRow dr in ds.Tables["Field"].Rows)
                {
                    var fld = new Field(dr);

                    if (dr.IsNotNull("field_id"))
                    {
                        var id = Convert.ToInt32(dr["field_id"]);

                        if (ds.Tables["OptionValue"] != null)
                        {
                            foreach (DataRow drOption in ds.Tables["OptionValue"].Select("field_id=" + id.ToString()))
                            {
                                try
                                {
                                    fld.OptionValues.Add(Convert.ToString(drOption["OptionValue_Text"]));
                                }
                                catch (Exception)
                                {
                                    fld.OptionValues.Add(Convert.ToString(drOption["OptionValue_Column"]));
                                    throw;
                                }
                            }
                        }

                        if (ds.Tables["ValidValue"] != null)
                        {
                            foreach (DataRow drValid in ds.Tables["ValidValue"].Select("field_id=" + id.ToString()))
                            {
                                try
                                {
                                    fld.ValidValues.Add(Convert.ToString(drValid["ValidValue_Text"]));
                                }
                                catch (Exception)
                                {
                                    fld.ValidValues.Add(Convert.ToString(drValid["ValidValue_Column"]));
                                    throw;
                                }
                            }
                        }
                    }

                    if (dr.IsNotNull("IncludeFile"))
                    {
                        var fi = new System.IO.FileInfo(xmlFile);

                        var includeFields = RecurseXmlDefinition(fi.Directory.FullName.AppendTrailingSeparator() +
                                                                 Convert.ToString(dr["IncludeFile"]));

                        foreach (var inclFld in includeFields.MessageFields)
                        {
                            inclFld.Name = inclFld.Name.Replace("#replace#", fld.Name);

                            if (!string.IsNullOrEmpty(inclFld.DependentField))
                            {
                                inclFld.DependentField = inclFld.DependentField.Replace("#replace#", fld.Name);
                            }

                            if (!string.IsNullOrEmpty(inclFld.DynamicLength))
                            {
                                inclFld.DynamicLength = inclFld.DynamicLength.Replace("#replace#", fld.Name);
                            }

                            inclFld.OptionValues.AddRange(fld.OptionValues);
                            inclFld.ValidValues.AddRange(fld.ValidValues);

                            if (!string.IsNullOrEmpty(fld.DependentField) &&
                                string.IsNullOrEmpty(inclFld.DependentField))
                            {
                                inclFld.DependentField = fld.DependentField;
                                inclFld.DependentValues = fld.DependentValues;
                                inclFld.ExclusiveDependency = fld.ExclusiveDependency;
                            }

                            if (!string.IsNullOrEmpty(fld.Repetitions) && string.IsNullOrEmpty(inclFld.Repetitions))
                            {
                                inclFld.Repetitions = fld.Repetitions;
                                inclFld.StaticRepetitions = fld.StaticRepetitions;
                            }

                            fields.MessageFields.Add(inclFld);
                        }
                    }
                    else
                    {
                        var len = Convert.ToString(dr["Length"]);

                        if (len.IsNumeric())
                        {
                            fld.Length = string.IsNullOrEmpty(fld.ParseUntil) ? Convert.ToInt32(len) : 1;
                        }
                        else
                        {
                            switch (len)
                            {
                                case Resources.Static.DOUBLE_LENGTH_ZMKS:
                                    fields.IsDynamic = true;
                                    fld.Length = Settings.Default.DoubleLengthZMKs ? 32 : 16;
                                    break;
                                case Resources.Static.CLEAR_PIN_LENGTH:
                                    fields.IsDynamic = true;
                                    fld.Length = Settings.Default.ClearPINLength + 1;
                                    break;
                                default:
                                    throw new InvalidOperationException(string.Format("Invalid length element [{0}]",
                                                                                      len));
                            }
                        }

                        fld.Type = (FieldType)Enum.Parse(typeof(FieldType), Convert.ToString(dr["Type"]), true);

                        fields.MessageFields.Add(fld);
                    }
                }

                return fields;
            }
        }
    }
}
