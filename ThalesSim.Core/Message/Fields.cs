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
    /// <summary>
    /// Used to contain a lis of field definitions.
    /// </summary>
    public class Fields
    {
        /// <summary>
        /// Get/set the list of field definitions.
        /// </summary>
        public List<Field> MessageFields { get; set; }

        /// <summary>
        /// Get/set whether this instance can dynamically change.
        /// This may happen when fields exist that have a dynamic
        /// length that corresponds to an internal variable.
        /// </summary>
        public bool IsDynamic { get; set; }

        /// <summary>
        /// Creates a new instance of this class without any fields.
        /// </summary>
        public Fields()
        {
            MessageFields = new List<Field>();
        }

        /// <summary>
        /// Returns a clone of this instance.
        /// </summary>
        /// <returns>Cloned copy of this instance.</returns>
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

        /// <summary>
        /// Creates a new instance of this class by
        /// reading an XML definitions file.
        /// </summary>
        /// <param name="xmlFile">XML definitions file.</param>
        /// <returns>Instance of this class.</returns>
        public static Fields ReadXmlDefinition (string xmlFile)
        {
            return RecurseXmlDefinition(Settings.Default.HostCommandDefinitions.AppendTrailingSeparator() + xmlFile);
        }

        /// <summary>
        /// Recursively process the XML definitions.
        /// </summary>
        /// <param name="xmlFile">File to read.</param>
        /// <returns>Instance of this class.</returns>
        private static Fields RecurseXmlDefinition (string xmlFile)
        {
            var fields = new Fields();

            using (var ds = new DataSet())
            {
                ds.ReadXml(xmlFile);

                // Check for empty definition.
                if (!ds.Tables.Contains("Field"))
                {
                    return fields;
                }

                foreach (DataRow dr in ds.Tables["Field"].Rows)
                {
                    // New field.
                    var fld = new Field(dr);

                    if (dr.IsNotNull("field_id"))
                    {
                        var id = Convert.ToInt32(dr["field_id"]);

                        // Get optional values.
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

                        // Get valid values.
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

                    // Check for include files.
                    if (dr.IsNotNull("IncludeFile"))
                    {
                        // We assume that the include files reside in the same directory.
                        var fi = new System.IO.FileInfo(xmlFile);

                        // Parse include file.
                        var includeFields = RecurseXmlDefinition(fi.Directory.FullName.AppendTrailingSeparator() +
                                                                 Convert.ToString(dr["IncludeFile"]));

                        foreach (var inclFld in includeFields.MessageFields)
                        {
                            // Take care to replace the #replace# tag with the field name.
                            inclFld.Name = inclFld.Name.Replace("#replace#", fld.Name);

                            // Do the same repelacement for the dependent field.
                            if (!string.IsNullOrEmpty(inclFld.DependentField))
                            {
                                inclFld.DependentField = inclFld.DependentField.Replace("#replace#", fld.Name);
                            }

                            // ...and the dynamic length field.
                            if (!string.IsNullOrEmpty(inclFld.DynamicLength))
                            {
                                inclFld.DynamicLength = inclFld.DynamicLength.Replace("#replace#", fld.Name);
                            }

                            // Option and valid values of the current field are appended.
                            inclFld.OptionValues.AddRange(fld.OptionValues);
                            inclFld.ValidValues.AddRange(fld.ValidValues);

                            // Note that if there are dependend field values for fld but
                            // not for inclFld we copy those from fld to inclFld as well.
                            if (!string.IsNullOrEmpty(fld.DependentField) &&
                                string.IsNullOrEmpty(inclFld.DependentField))
                            {
                                inclFld.DependentField = fld.DependentField;
                                inclFld.DependentValues = fld.DependentValues;
                                inclFld.ExclusiveDependency = fld.ExclusiveDependency;
                            }

                            // Same as above for repetitions.
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
                        // Get length.
                        var len = Convert.ToString(dr["Length"]);

                        if (len.IsNumeric())
                        {
                            // When we want to parse until we find a value, we set
                            // a length of 1. This is because we want to look for a
                            // value of a single-character.
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

                        // Get field type.
                        fld.Type = (FieldType)Enum.Parse(typeof(FieldType), Convert.ToString(dr["Type"]), true);

                        // Add this field.
                        fields.MessageFields.Add(fld);
                    }
                }

                return fields;
            }
        }
    }
}
