﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Text;
using System.Text.RegularExpressions;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  /// <summary>
  /// Serializes a string to XML by embedding into a CDATA block.
  /// </summary>
  public sealed class String2XmlSerializer : PrimitiveXmlSerializerBase<string> {

    /// <summary>
    /// Formats the specified string.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <returns>An XmlString that embeds the string s in a CDATA section.</returns>
    public override XmlString Format(string s) {
      StringBuilder sb = new StringBuilder();
      sb.Append("<![CDATA[");
      sb.Append(s.Replace("]]>", "]]]]><![CDATA[>"));
      sb.Append("]]>");
      s = special.Replace(sb.ToString(), m => ToBase64Tag(m.Value));
      return new XmlString(s);
    }

    private static readonly Regex re = new Regex(@"<!\[CDATA\[((?:[^]]|\](?!\]>))*)\]\]>|<Base64>([^<]*)</Base64>", RegexOptions.Singleline);
    private static readonly Regex special = new Regex(@"[\x00-\x08\x0b\x0c\x0e-\x1f]+", RegexOptions.Singleline);

    private static string ToBase64Tag(string s) {
      return new StringBuilder()
        .Append("]]><Base64>")
        .Append(Convert.ToBase64String(Encoding.ASCII.GetBytes(s)))
        .Append("</Base64><![CDATA[")
        .ToString();
    }

    /// <summary>
    /// Parses the specified XmlString into a string.
    /// </summary>
    /// <param name="x">The XMLString.</param>
    /// <returns>The plain string contained in the XML CDATA section.</returns>
    public override string Parse(XmlString x) {
      StringBuilder sb = new StringBuilder();
      foreach (Match m in re.Matches(x.Data)) {
        if (m.Groups[1].Success)
          sb.Append(m.Groups[1].Value);
        else if (m.Groups[2].Success) {
          sb.Append(Encoding.ASCII.GetString(Convert.FromBase64String(m.Groups[2].Value)));
        }
      }
      string result = sb.ToString();
      if (result.Length == 0 && x.Data.Length > 0 && !x.Data.Equals("<![CDATA[]]>"))
        throw new PersistenceException("Invalid CDATA section during string parsing.");
      return result;
    }
  }
}