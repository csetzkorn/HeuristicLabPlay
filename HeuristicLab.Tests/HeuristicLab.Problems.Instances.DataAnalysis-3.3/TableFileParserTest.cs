#region License Information
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.Instances.DataAnalysis.Tests {
  [TestClass()]
  public class TableFileParserTest {
    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseCSV() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"0.00, 0.00, 0.00, 3.14
0.00, 0.00, 0.00, 0.00
0.00, 0.00, 0.00, 0.00
0.00, 0.00, 0.00, 0.00
0.00, 0.00, 0.00, 0.00
0.00, 0.00, 0.00, 0.00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual(parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseCSVWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"x01, x02, x03, x04
0.00, 0.00, 0.00, 3.14
0.00, 0.00, 0.00, 0.00
0.00, 0.00, 0.00, 0.00
0.00, 0.00, 0.00, 0.00
0.00, 0.00, 0.00, 0.00
0.00, 0.00, 0.00, 0.00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual(parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseGermanCSV() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"0,00; 0,00; 0,00; 3,14
0,00; 0,00; 0,00; 0,00
0,00; 0,00; 0,00; 0,00
0,00; 0,00; 0,00; 0,00
0,00; 0,00; 0,00; 0,00
0,00; 0,00; 0,00; 0,00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual(parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseGermanCSVWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"x01; x02; x03; x04
0,00; 0,00; 0,00; 3,14
0,00; 0,00; 0,00; 0,00
0,00; 0,00; 0,00; 0,00
0,00; 0,00; 0,00; 0,00
0,00; 0,00; 0,00; 0,00
0,00; 0,00; 0,00; 0,00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual(parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseGermanCSVWithoutCommas() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"0; 0; 0; 3
0; 0; 0; 0
0; 0; 0; 0
0; 0; 0; 0
0; 0; 0; 0
0; 0; 0; 0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseGermanCSVWithoutCommasWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"x01; x02; x03; x04
0; 0; 0; 3
0; 0; 0; 0
0; 0; 0; 0
0; 0; 0; 0
0; 0; 0; 0
0; 0; 0; 0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseEnglishCSVWithoutCommas() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"0, 0, 0, 3
0, 0, 0, 0
0, 0, 0, 0
0, 0, 0, 0
0, 0, 0, 0
0, 0, 0, 0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseEnglishCSVWithoutCommasWithoutSpace() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"0,0,0,3
0,0,0,0
0,0,0,0
0,0,0,0
0,0,0,0
0,0,0,0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseEnglishCSVWithoutCommasWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"x01, x02, x03, x04
0, 0, 0, 3
0, 0, 0, 0
0, 0, 0, 0
0, 0, 0, 0
0, 0, 0, 0
0, 0, 0, 0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseEnglishCSVWithoutCommasWithoutSpacesWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"x01,x02,x03,x04
0,0,0,3
0,0,0,0
0,0,0,0
0,0,0,0
0,0,0,0
0,0,0,0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }


    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseGermanTabSeparated() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      "0,00\t 0,00\t 0,00\t 3,14" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseGermanTabSeparatedWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      "x01\t x02\t x03\t x04" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 3,14" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseEnglishTabSeparated() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      "0.00\t 0.00\t 0.00\t 3.14" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseEnglishTabSeparatedWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      "x01\t x02\t x03\t x04" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 3.14" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseTabSeparatedWithoutCommas() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      "0\t 0\t 0\t 3" + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      "0\t 0\t 0\t 0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseTabSeparatedWithoutCommasWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      "x01\t x02\t x03\t x04" + Environment.NewLine +
      "0\t 0\t 0\t 3" + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      "0\t 0\t 0\t 0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseWithEmtpyLines() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      "x01\t x02\t x03\t x04" + Environment.NewLine +
      "0\t 0\t 0\t 3" + Environment.NewLine +
      Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      " " + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine + Environment.NewLine);
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(4, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseGermanSpaceSeparated() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"0,00 0,00 0,00 3,14
0,00 0,00 0,00 0,00
0,00 0,00 0,00 0,00
0,00 0,00 0,00 0,00
0,00 0,00 0,00 0,00
0,00 0,00 0,00 0,00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseGermanSpaceSeparatedWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"x01 x02 x03 x04
0,00 0,00 0,00 3,14
0,00 0,00 0,00 0,00
0,00 0,00 0,00 0,00
0,00 0,00 0,00 0,00
0,00 0,00 0,00 0,00
0,00 0,00 0,00 0,00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseEnglishSpaceSeparated() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"0.00 0.00 0.00 3.14
0.00 0.00 0.00 0.00
0.00 0.00 0.00 0.00
0.00 0.00 0.00 0.00
0.00 0.00 0.00 0.00
0.00 0.00 0.00 0.00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseEnglishSpaceSeparatedWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"x01 x02 x03 x04
0.00 0.00 0.00 3.14
0.00 0.00 0.00 0.00
0.00 0.00 0.00 0.00
0.00 0.00 0.00 0.00
0.00 0.00 0.00 0.00
0.00 0.00 0.00 0.00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseEnglishSpaceSeparatedWithNamesManyColumns() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"id,y,x00001,x00002,x00003,x00004,x00005,x00006,x00007,x00008,x00009,x00010,x00011,x00012,x00013,x00014,x00015,x00016,x00017,x00018,x00019,x00020,x00021,x00022,x00023,x00024,x00025,x00026,x00027,x00028,x00029,x00030,x00031,x00032,x00033,x00034,x00035,x00036,x00037,x00038,x00039,x00040,x00041,x00042,x00043,x00044,x00045,x00046,x00047,x00048,x00049,x00050,x00051,x00052,x00053,x00054,x00055,x00056,x00057,x00058,x00059,x00060,x00061,x00062,x00063,x00064,x00065,x00066,x00067,x00068,x00069,x00070,x00071,x00072,x00073,x00074,x00075,x00076,x00077,x00078,x00079,x00080,x00081,x00082,x00083,x00084,x00085,x00086,x00087,x00088,x00089,x00090,x00091,x00092,x00093,x00094,x00095,x00096,x00097,x00098,x00099,x00100,x00101,x00102,x00103,x00104,x00105,x00106,x00107,x00108,x00109,x00110,x00111,x00112,x00113,x00114,x00115,x00116,x00117,x00118,x00119,x00120,x00121,x00122,x00123,x00124,x00125,x00126,x00127,x00128,x00129,x00130,x00131,x00132,x00133,x00134,x00135,x00136,x00137,x00138,x00139,x00140,x00141,x00142,x00143,x00144,x00145,x00146,x00147,x00148,x00149,x00150,x00151,x00152,x00153,x00154,x00155,x00156,x00157,x00158,x00159,x00160,x00161,x00162,x00163,x00164,x00165,x00166,x00167,x00168,x00169,x00170,x00171,x00172,x00173,x00174,x00175,x00176,x00177,x00178,x00179,x00180,x00181,x00182,x00183,x00184,x00185,x00186,x00187,x00188,x00189,x00190,x00191,x00192,x00193,x00194,x00195,x00196,x00197,x00198,x00199,x00200,x00201,x00202,x00203,x00204,x00205,x00206,x00207,x00208,x00209,x00210,x00211,x00212,x00213,x00214,x00215,x00216,x00217,x00218,x00219,x00220,x00221,x00222,x00223,x00224,x00225,x00226,x00227,x00228,x00229,x00230,x00231,x00232,x00233,x00234,x00235,x00236,x00237,x00238,x00239,x00240,x00241,x00242,x00243,x00244,x00245,x00246,x00247,x00248,x00249,x00250,x00251,x00252,x00253,x00254,x00255,x00256,x00257,x00258,x00259,x00260,x00261,x00262,x00263,x00264,x00265,x00266,x00267,x00268,x00269,x00270,x00271,x00272,x00273,x00274,x00275,x00276,x00277,x00278,x00279,x00280,x00281,x00282,x00283,x00284,x00285,x00286,x00287,x00288,x00289,x00290,x00291,x00292,x00293,x00294,x00295,x00296,x00297,x00298,x00299,x00300,x00301,x00302,x00303,x00304,x00305,x00306,x00307,x00308,x00309,x00310,x00311,x00312,x00313,x00314,x00315,x00316,x00317,x00318,x00319,x00320,x00321,x00322,x00323,x00324,x00325,x00326,x00327,x00328,x00329,x00330,x00331,x00332,x00333,x00334,x00335,x00336,x00337,x00338,x00339,x00340,x00341,x00342,x00343,x00344,x00345,x00346,x00347,x00348,x00349,x00350,x00351,x00352,x00353,x00354,x00355,x00356,x00357,x00358,x00359,x00360,x00361,x00362,x00363,x00364,x00365,x00366,x00367,x00368,x00369,x00370,x00371,x00372,x00373,x00374,x00375,x00376,x00377,x00378,x00379,x00380,x00381,x00382,x00383,x00384,x00385,x00386,x00387,x00388,x00389,x00390,x00391,x00392,x00393,x00394,x00395,x00396,x00397,x00398,x00399,x00400,x00401,x00402,x00403,x00404,x00405,x00406,x00407,x00408,x00409,x00410,x00411,x00412,x00413,x00414,x00415,x00416,x00417,x00418,x00419,x00420,x00421,x00422,x00423,x00424,x00425,x00426,x00427,x00428,x00429,x00430,x00431,x00432,x00433,x00434,x00435,x00436,x00437,x00438,x00439,x00440,x00441,x00442,x00443,x00444,x00445,x00446,x00447,x00448,x00449,x00450,x00451,x00452,x00453,x00454,x00455,x00456,x00457,x00458,x00459,x00460,x00461,x00462,x00463,x00464,x00465,x00466,x00467,x00468,x00469,x00470,x00471,x00472,x00473,x00474,x00475,x00476,x00477,x00478,x00479,x00480,x00481,x00482,x00483,x00484,x00485,x00486,x00487,x00488,x00489,x00490,x00491,x00492,x00493,x00494,x00495,x00496,x00497,x00498,x00499,x00500,x00501,x00502,x00503,x00504,x00505,x00506,x00507,x00508,x00509,x00510,x00511,x00512,x00513,x00514,x00515,x00516,x00517,x00518,x00519,x00520,x00521,x00522,x00523,x00524,x00525,x00526,x00527,x00528,x00529,x00530,x00531,x00532,x00533,x00534,x00535,x00536,x00537,x00538,x00539,x00540,x00541,x00542,x00543,x00544,x00545,x00546,x00547,x00548,x00549,x00550,x00551,x00552,x00553,x00554,x00555,x00556,x00557,x00558,x00559,x00560,x00561,x00562,x00563,x00564,x00565,x00566,x00567,x00568,x00569,x00570,x00571,x00572,x00573,x00574,x00575,x00576,x00577,x00578,x00579,x00580,x00581,x00582,x00583,x00584,x00585,x00586,x00587,x00588,x00589,x00590,x00591,x00592,x00593,x00594,x00595,x00596,x00597,x00598,x00599,x00600,x00601,x00602,x00603,x00604,x00605,x00606,x00607,x00608,x00609,x00610,x00611,x00612,x00613,x00614,x00615,x00616,x00617,x00618,x00619,x00620,x00621,x00622,x00623,x00624,x00625,x00626,x00627,x00628,x00629,x00630,x00631,x00632,x00633,x00634,x00635,x00636,x00637,x00638,x00639,x00640,x00641,x00642,x00643,x00644,x00645,x00646,x00647,x00648,x00649,x00650,x00651,x00652,x00653,x00654,x00655,x00656,x00657,x00658,x00659,x00660,x00661,x00662,x00663,x00664,x00665,x00666,x00667,x00668,x00669,x00670,x00671,x00672,x00673,x00674,x00675,x00676,x00677,x00678,x00679,x00680,x00681,x00682,x00683,x00684,x00685,x00686,x00687,x00688,x00689,x00690,x00691,x00692,x00693,x00694,x00695,x00696,x00697,x00698,x00699,x00700,x00701,x00702,x00703,x00704,x00705,x00706,x00707,x00708,x00709,x00710,x00711,x00712,x00713,x00714,x00715,x00716,x00717,x00718,x00719,x00720,x00721,x00722,x00723,x00724,x00725,x00726,x00727,x00728,x00729,x00730,x00731,x00732,x00733,x00734,x00735,x00736,x00737,x00738,x00739,x00740,x00741,x00742,x00743,x00744,x00745,x00746,x00747,x00748,x00749,x00750,x00751,x00752,x00753,x00754,x00755,x00756,x00757,x00758,x00759,x00760,x00761,x00762,x00763,x00764,x00765,x00766,x00767,x00768,x00769,x00770,x00771,x00772,x00773,x00774,x00775,x00776,x00777,x00778,x00779,x00780,x00781,x00782,x00783,x00784,x00785,x00786,x00787,x00788,x00789,x00790,x00791,x00792,x00793,x00794,x00795,x00796,x00797,x00798,x00799,x00800,x00801,x00802,x00803,x00804,x00805,x00806,x00807,x00808,x00809,x00810,x00811,x00812,x00813,x00814,x00815,x00816,x00817,x00818,x00819,x00820,x00821,x00822,x00823,x00824,x00825,x00826,x00827,x00828,x00829,x00830,x00831,x00832,x00833,x00834,x00835,x00836,x00837,x00838,x00839,x00840,x00841,x00842,x00843,x00844,x00845,x00846,x00847,x00848,x00849,x00850,x00851,x00852,x00853,x00854,x00855,x00856,x00857,x00858,x00859,x00860,x00861,x00862,x00863,x00864,x00865,x00866,x00867,x00868,x00869,x00870,x00871,x00872,x00873,x00874,x00875,x00876,x00877,x00878,x00879,x00880,x00881,x00882,x00883,x00884,x00885,x00886,x00887,x00888,x00889,x00890,x00891,x00892,x00893,x00894,x00895,x00896,x00897,x00898,x00899,x00900,x00901,x00902,x00903,x00904,x00905,x00906,x00907,x00908,x00909,x00910,x00911,x00912,x00913,x00914,x00915,x00916,x00917,x00918,x00919,x00920,x00921,x00922,x00923,x00924,x00925,x00926,x00927,x00928,x00929,x00930,x00931,x00932,x00933,x00934,x00935,x00936,x00937,x00938,x00939,x00940,x00941,x00942,x00943,x00944,x00945,x00946,x00947,x00948,x00949,x00950,x00951,x00952,x00953,x00954,x00955,x00956,x00957,x00958,x00959,x00960,x00961,x00962,x00963,x00964,x00965,x00966,x00967,x00968,x00969,x00970,x00971,x00972,x00973,x00974,x00975,x00976,x00977,x00978,x00979,x00980,x00981,x00982,x00983,x00984,x00985,x00986,x00987,x00988,x00989,x00990,x00991,x00992,x00993,x00994,x00995,x00996,x00997,x00998,x00999,x01000,x01001,x01002,x01003,x01004,x01005,x01006,x01007,x01008,x01009,x01010,x01011,x01012,x01013,x01014,x01015,x01016,x01017,x01018,x01019,x01020,x01021,x01022,x01023,x01024,x01025,x01026,x01027,x01028,x01029,x01030,x01031,x01032,x01033,x01034,x01035,x01036,x01037,x01038,x01039,x01040,x01041,x01042,x01043,x01044,x01045,x01046,x01047,x01048,x01049,x01050,x01051,x01052,x01053,x01054,x01055,x01056,x01057,x01058,x01059,x01060,x01061,x01062,x01063,x01064,x01065,x01066,x01067,x01068,x01069,x01070,x01071,x01072,x01073,x01074,x01075,x01076,x01077,x01078,x01079,x01080,x01081,x01082,x01083,x01084,x01085,x01086,x01087,x01088,x01089,x01090,x01091,x01092,x01093,x01094,x01095,x01096,x01097,x01098,x01099,x01100,x01101,x01102,x01103,x01104,x01105,x01106,x01107,x01108,x01109,x01110,x01111,x01112,x01113,x01114,x01115,x01116,x01117,x01118,x01119,x01120,x01121,x01122,x01123,x01124,x01125,x01126,x01127,x01128,x01129,x01130,x01131,x01132,x01133,x01134,x01135,x01136,x01137,x01138,x01139,x01140,x01141,x01142,x01143,x01144,x01145,x01146,x01147,x01148,x01149,x01150,x01151,x01152,x01153,x01154,x01155,x01156,x01157,x01158,x01159,x01160,x01161,x01162,x01163,x01164,x01165,x01166,x01167,x01168,x01169,x01170,x01171,x01172,x01173,x01174,x01175,x01176,x01177,x01178,x01179,x01180,x01181,x01182,x01183,x01184,x01185,x01186,x01187,x01188,x01189,x01190,x01191,x01192,x01193,x01194,x01195,x01196,x01197,x01198,x01199,x01200,x01201,x01202,x01203,x01204,x01205,x01206,x01207,x01208,x01209,x01210,x01211,x01212,x01213,x01214,x01215,x01216,x01217,x01218,x01219,x01220,x01221,x01222,x01223,x01224,x01225,x01226,x01227,x01228,x01229,x01230,x01231,x01232,x01233,x01234,x01235,x01236,x01237,x01238,x01239,x01240,x01241,x01242,x01243,x01244,x01245,x01246,x01247,x01248,x01249,x01250,x01251,x01252,x01253,x01254,x01255,x01256,x01257,x01258,x01259,x01260,x01261,x01262,x01263,x01264,x01265,x01266,x01267,x01268,x01269,x01270,x01271,x01272,x01273,x01274,x01275,x01276,x01277,x01278,x01279,x01280,x01281,x01282,x01283,x01284,x01285,x01286,x01287,x01288,x01289,x01290,x01291,x01292,x01293,x01294,x01295,x01296,x01297,x01298,x01299,x01300,x01301,x01302,x01303,x01304,x01305,x01306,x01307,x01308,x01309,x01310,x01311,x01312,x01313,x01314,x01315,x01316,x01317,x01318,x01319,x01320,x01321,x01322,x01323,x01324,x01325,x01326,x01327,x01328,x01329,x01330,x01331,x01332,x01333,x01334,x01335,x01336,x01337,x01338,x01339,x01340,x01341,x01342,x01343,x01344,x01345,x01346,x01347,x01348,x01349,x01350,x01351,x01352,x01353,x01354,x01355,x01356,x01357,x01358,x01359,x01360,x01361,x01362,x01363,x01364,x01365,x01366,x01367,x01368,x01369,x01370,x01371,x01372,x01373,x01374,x01375,x01376,x01377,x01378,x01379,x01380,x01381,x01382,x01383,x01384,x01385,x01386,x01387,x01388,x01389,x01390,x01391,x01392,x01393,x01394,x01395,x01396,x01397,x01398,x01399,x01400,x01401,x01402,x01403,x01404,x01405,x01406,x01407,x01408,x01409,x01410,x01411,x01412,x01413,x01414,x01415,x01416,x01417,x01418,x01419,x01420,x01421,x01422,x01423,x01424,x01425,x01426,x01427,x01428,x01429,x01430,x01431,x01432,x01433,x01434,x01435,x01436,x01437,x01438,x01439,x01440,x01441,x01442,x01443,x01444,x01445,x01446,x01447,x01448,x01449,x01450,x01451,x01452,x01453,x01454,x01455,x01456,x01457,x01458,x01459,x01460,x01461,x01462,x01463,x01464,x01465,x01466,x01467,x01468,x01469,x01470,x01471,x01472,x01473,x01474,x01475,x01476,x01477,x01478,x01479,x01480,x01481,x01482,x01483,x01484,x01485,x01486,x01487,x01488,x01489,x01490,x01491,x01492,x01493,x01494,x01495,x01496,x01497,x01498,x01499,x01500,x01501,x01502,x01503,x01504,x01505,x01506,x01507,x01508,x01509,x01510,x01511,x01512,x01513,x01514,x01515,x01516,x01517,x01518,x01519,x01520,x01521,x01522,x01523,x01524,x01525,x01526,x01527,x01528,x01529,x01530,x01531,x01532,x01533,x01534,x01535,x01536,x01537,x01538,x01539,x01540,x01541,x01542,x01543,x01544,x01545,x01546,x01547,x01548,x01549,x01550,x01551,x01552,x01553,x01554,x01555,x01556,x01557,x01558,x01559,x01560,x01561,x01562,x01563,x01564,x01565,x01566,x01567,x01568,x01569,x01570,x01571,x01572,x01573,x01574,x01575,x01576,x01577,x01578,x01579,x01580,x01581,x01582,x01583,x01584,x01585,x01586,x01587,x01588,x01589,x01590,x01591,x01592,x01593,x01594,x01595,x01596,x01597,x01598,x01599,x01600,x01601,x01602,x01603,x01604,x01605,x01606,x01607,x01608,x01609,x01610,x01611,x01612,x01613,x01614,x01615,x01616,x01617,x01618,x01619,x01620,x01621,x01622,x01623,x01624,x01625,x01626,x01627,x01628,x01629,x01630,x01631,x01632,x01633,x01634,x01635,x01636,x01637,x01638,x01639,x01640,x01641,x01642,x01643,x01644,x01645,x01646,x01647,x01648,x01649,x01650,x01651,x01652,x01653,x01654,x01655,x01656,x01657,x01658,x01659,x01660,x01661,x01662,x01663,x01664,x01665,x01666,x01667,x01668,x01669,x01670,x01671,x01672,x01673,x01674,x01675,x01676,x01677,x01678,x01679,x01680,x01681,x01682,x01683,x01684,x01685,x01686,x01687,x01688,x01689,x01690,x01691,x01692,x01693,x01694,x01695,x01696,x01697,x01698,x01699,x01700,x01701,x01702,x01703,x01704,x01705,x01706,x01707,x01708,x01709,x01710,x01711,x01712,x01713,x01714,x01715,x01716,x01717,x01718,x01719,x01720,x01721,x01722,x01723,x01724,x01725,x01726,x01727,x01728,x01729,x01730,x01731,x01732,x01733,x01734,x01735,x01736,x01737,x01738,x01739,x01740,x01741,x01742,x01743,x01744,x01745,x01746,x01747,x01748,x01749,x01750,x01751,x01752,x01753,x01754,x01755,x01756,x01757,x01758,x01759,x01760,x01761,x01762,x01763,x01764,x01765,x01766,x01767,x01768,x01769,x01770,x01771,x01772,x01773,x01774,x01775,x01776,x01777,x01778,x01779,x01780,x01781,x01782,x01783,x01784,x01785,x01786,x01787,x01788,x01789,x01790,x01791,x01792,x01793,x01794,x01795,x01796,x01797,x01798,x01799,x01800,x01801,x01802,x01803,x01804,x01805,x01806,x01807,x01808,x01809,x01810,x01811,x01812,x01813,x01814,x01815,x01816,x01817,x01818,x01819,x01820,x01821,x01822,x01823,x01824,x01825,x01826,x01827,x01828,x01829,x01830,x01831,x01832,x01833,x01834,x01835,x01836,x01837,x01838,x01839,x01840,x01841,x01842,x01843,x01844,x01845,x01846,x01847,x01848,x01849,x01850,x01851,x01852,x01853,x01854,x01855,x01856,x01857,x01858,x01859,x01860,x01861,x01862,x01863,x01864,x01865,x01866,x01867,x01868,x01869,x01870,x01871,x01872,x01873,x01874,x01875,x01876,x01877,x01878,x01879,x01880,x01881,x01882,x01883,x01884,x01885,x01886,x01887,x01888,x01889,x01890,x01891,x01892,x01893,x01894,x01895,x01896,x01897,x01898,x01899,x01900,x01901,x01902,x01903,x01904,x01905,x01906,x01907,x01908,x01909,x01910,x01911,x01912,x01913,x01914,x01915,x01916,x01917,x01918,x01919,x01920,x01921,x01922,x01923,x01924,x01925,x01926,x01927,x01928,x01929,x01930,x01931,x01932,x01933,x01934,x01935,x01936,x01937,x01938,x01939,x01940,x01941,x01942,x01943,x01944,x01945,x01946,x01947,x01948,x01949,x01950,x01951,x01952,x01953,x01954,x01955,x01956,x01957,x01958,x01959,x01960,x01961,x01962,x01963,x01964,x01965,x01966,x01967,x01968,x01969,x01970,x01971,x01972,x01973,x01974,x01975,x01976,x01977,x01978,x01979,x01980,x01981,x01982,x01983,x01984,x01985,x01986,x01987,x01988,x01989,x01990,x01991,x01992,x01993,x01994,x01995,x01996,x01997,x01998,x01999,x02000,x02001,x02002,x02003,x02004,x02005,x02006,x02007,x02008,x02009,x02010,x02011,x02012,x02013,x02014,x02015,x02016,x02017,x02018,x02019,x02020,x02021,x02022,x02023,x02024,x02025,x02026,x02027,x02028,x02029,x02030,x02031,x02032,x02033,x02034,x02035,x02036,x02037,x02038,x02039,x02040,x02041,x02042,x02043,x02044,x02045,x02046,x02047,x02048,x02049,x02050,x02051,x02052,x02053,x02054,x02055,x02056,x02057,x02058,x02059,x02060,x02061,x02062,x02063,x02064,x02065,x02066,x02067,x02068,x02069,x02070,x02071,x02072,x02073,x02074,x02075,x02076,x02077,x02078,x02079,x02080,x02081,x02082,x02083,x02084,x02085,x02086,x02087,x02088,x02089,x02090,x02091,x02092,x02093,x02094,x02095,x02096,x02097,x02098,x02099,x02100,x02101,x02102,x02103,x02104,x02105,x02106,x02107,x02108,x02109,x02110,x02111,x02112,x02113,x02114,x02115,x02116,x02117,x02118,x02119,x02120,x02121,x02122,x02123,x02124,x02125,x02126,x02127,x02128,x02129,x02130,x02131,x02132,x02133,x02134,x02135,x02136,x02137,x02138,x02139,x02140,x02141,x02142,x02143,x02144,x02145,x02146,x02147,x02148,x02149,x02150,x02151,x02152,x02153,x02154,x02155,x02156,x02157,x02158,x02159,x02160,x02161,x02162,x02163,x02164,x02165,x02166,x02167,x02168,x02169,x02170,x02171,x02172,x02173,x02174,x02175,x02176,x02177,x02178,x02179,x02180,x02181,x02182,x02183,x02184,x02185,x02186,x02187,x02188,x02189,x02190,x02191,x02192,x02193,x02194,x02195,x02196,x02197,x02198,x02199,x02200,x02201,x02202,x02203,x02204,x02205,x02206,x02207,x02208,x02209,x02210,x02211,x02212,x02213,x02214,x02215,x02216,x02217,x02218,x02219,x02220,x02221,x02222,x02223,x02224,x02225,x02226,x02227,x02228,x02229,x02230,x02231,x02232,x02233,x02234,x02235,x02236,x02237,x02238,x02239,x02240,x02241,x02242,x02243,x02244,x02245,x02246,x02247,x02248,x02249,x02250,x02251,x02252,x02253,x02254,x02255,x02256,x02257,x02258,x02259,x02260,x02261,x02262,x02263,x02264,x02265,x02266,x02267,x02268,x02269,x02270,x02271,x02272,x02273,x02274,x02275,x02276,x02277,x02278,x02279,x02280,x02281,x02282,x02283,x02284,x02285,x02286,x02287,x02288,x02289,x02290,x02291,x02292,x02293,x02294,x02295,x02296,x02297,x02298,x02299,x02300,x02301,x02302,x02303,x02304,x02305,x02306,x02307,x02308,x02309,x02310,x02311,x02312,x02313,x02314,x02315,x02316,x02317,x02318,x02319,x02320,x02321,x02322,x02323,x02324,x02325,x02326,x02327,x02328,x02329,x02330,x02331,x02332,x02333,x02334,x02335,x02336,x02337,x02338,x02339,x02340,x02341,x02342,x02343,x02344,x02345,x02346,x02347,x02348,x02349,x02350,x02351,x02352,x02353,x02354,x02355,x02356,x02357,x02358,x02359,x02360,x02361,x02362,x02363,x02364,x02365,x02366,x02367,x02368,x02369,x02370,x02371,x02372,x02373,x02374,x02375,x02376,x02377,x02378,x02379,x02380,x02381,x02382,x02383,x02384,x02385,x02386,x02387,x02388,x02389,x02390,x02391,x02392,x02393,x02394,x02395,x02396,x02397,x02398,x02399,x02400,x02401,x02402,x02403,x02404,x02405,x02406,x02407,x02408,x02409,x02410,x02411,x02412,x02413,x02414,x02415,x02416,x02417,x02418,x02419,x02420,x02421,x02422,x02423,x02424,x02425,x02426,x02427,x02428,x02429,x02430,x02431,x02432,x02433,x02434,x02435,x02436,x02437,x02438,x02439,x02440,x02441,x02442,x02443,x02444,x02445,x02446,x02447,x02448,x02449,x02450,x02451,x02452,x02453,x02454,x02455,x02456,x02457,x02458,x02459,x02460,x02461,x02462,x02463,x02464,x02465,x02466,x02467,x02468,x02469,x02470,x02471,x02472,x02473,x02474,x02475,x02476,x02477,x02478,x02479,x02480,x02481,x02482,x02483,x02484,x02485,x02486,x02487,x02488,x02489,x02490,x02491,x02492,x02493,x02494,x02495,x02496,x02497,x02498,x02499,x02500,x02501,x02502,x02503,x02504,x02505,x02506,x02507,x02508,x02509,x02510,x02511,x02512,x02513,x02514,x02515,x02516,x02517,x02518,x02519,x02520,x02521,x02522,x02523,x02524,x02525,x02526,x02527,x02528,x02529,x02530,x02531,x02532,x02533,x02534,x02535,x02536,x02537,x02538,x02539,x02540,x02541,x02542,x02543,x02544,x02545,x02546,x02547,x02548,x02549,x02550,x02551,x02552,x02553,x02554,x02555,x02556,x02557,x02558,x02559,x02560,x02561,x02562,x02563,x02564,x02565,x02566,x02567,x02568,x02569,x02570,x02571,x02572,x02573,x02574,x02575,x02576,x02577,x02578,x02579,x02580,x02581,x02582,x02583,x02584,x02585,x02586,x02587,x02588,x02589,x02590,x02591,x02592,x02593,x02594,x02595,x02596,x02597,x02598,x02599,x02600,x02601,x02602,x02603,x02604,x02605,x02606,x02607,x02608,x02609,x02610,x02611,x02612,x02613,x02614,x02615,x02616,x02617,x02618,x02619,x02620,x02621,x02622,x02623,x02624,x02625,x02626,x02627,x02628,x02629,x02630,x02631,x02632,x02633,x02634,x02635,x02636,x02637,x02638,x02639,x02640,x02641,x02642,x02643,x02644,x02645,x02646,x02647,x02648,x02649,x02650,x02651,x02652,x02653,x02654,x02655,x02656,x02657,x02658,x02659,x02660,x02661,x02662,x02663,x02664,x02665,x02666,x02667,x02668,x02669,x02670,x02671,x02672,x02673,x02674,x02675,x02676,x02677,x02678,x02679,x02680,x02681,x02682,x02683,x02684,x02685,x02686,x02687,x02688,x02689,x02690,x02691,x02692,x02693,x02694,x02695,x02696,x02697,x02698,x02699,x02700,x02701,x02702,x02703,x02704,x02705,x02706,x02707,x02708,x02709,x02710,x02711,x02712,x02713,x02714,x02715,x02716,x02717,x02718,x02719,x02720,x02721,x02722,x02723,x02724,x02725,x02726,x02727,x02728,x02729,x02730,x02731,x02732,x02733,x02734,x02735,x02736,x02737,x02738,x02739,x02740,x02741,x02742,x02743,x02744,x02745,x02746,x02747,x02748,x02749,x02750,x02751,x02752,x02753,x02754,x02755,x02756,x02757,x02758,x02759,x02760,x02761,x02762,x02763,x02764,x02765,x02766,x02767,x02768,x02769,x02770,x02771,x02772,x02773,x02774,x02775,x02776,x02777,x02778,x02779,x02780,x02781,x02782,x02783,x02784,x02785,x02786,x02787,x02788,x02789,x02790,x02791,x02792,x02793,x02794,x02795,x02796,x02797,x02798,x02799,x02800,x02801,x02802,x02803,x02804,x02805,x02806,x02807,x02808,x02809,x02810,x02811,x02812,x02813,x02814,x02815,x02816,x02817,x02818,x02819,x02820,x02821,x02822,x02823,x02824,x02825,x02826,x02827,x02828,x02829,x02830,x02831,x02832,x02833,x02834,x02835,x02836,x02837,x02838,x02839,x02840,x02841,x02842,x02843,x02844,x02845,x02846,x02847,x02848,x02849,x02850,x02851,x02852,x02853,x02854,x02855,x02856,x02857,x02858,x02859,x02860,x02861,x02862,x02863,x02864,x02865,x02866,x02867,x02868,x02869,x02870,x02871,x02872,x02873,x02874,x02875,x02876,x02877,x02878,x02879,x02880,x02881,x02882,x02883,x02884,x02885,x02886,x02887,x02888,x02889,x02890,x02891,x02892,x02893,x02894,x02895,x02896,x02897,x02898,x02899,x02900,x02901,x02902,x02903,x02904,x02905,x02906,x02907,x02908,x02909,x02910,x02911,x02912,x02913,x02914,x02915,x02916,x02917,x02918,x02919,x02920,x02921,x02922,x02923,x02924,x02925,x02926,x02927,x02928,x02929,x02930,x02931,x02932,x02933,x02934,x02935,x02936,x02937,x02938,x02939,x02940,x02941,x02942,x02943,x02944,x02945,x02946,x02947,x02948,x02949,x02950,x02951,x02952,x02953,x02954,x02955,x02956,x02957,x02958,x02959,x02960,x02961,x02962,x02963,x02964,x02965,x02966,x02967,x02968,x02969,x02970,x02971,x02972,x02973,x02974,x02975,x02976,x02977,x02978,x02979,x02980,x02981,x02982,x02983,x02984,x02985,x02986,x02987,x02988,x02989,x02990,x02991,x02992,x02993,x02994,x02995,x02996,x02997,x02998,x02999,x03000,x03001,x03002,x03003,x03004,x03005,x03006,x03007,x03008,x03009,x03010,x03011,x03012,x03013,x03014,x03015,x03016,x03017,x03018,x03019,x03020,x03021,x03022,x03023,x03024,x03025,x03026,x03027,x03028,x03029,x03030,x03031,x03032,x03033,x03034,x03035,x03036,x03037,x03038,x03039,x03040,x03041,x03042,x03043,x03044,x03045,x03046,x03047,x03048,x03049,x03050,x03051,x03052,x03053,x03054,x03055,x03056,x03057,x03058,x03059,x03060,x03061,x03062,x03063,x03064,x03065,x03066,x03067,x03068,x03069,x03070,x03071,x03072,x03073,x03074,x03075,x03076,x03077,x03078,x03079,x03080,x03081,x03082,x03083,x03084,x03085,x03086,x03087,x03088,x03089,x03090,x03091,x03092,x03093,x03094,x03095,x03096,x03097,x03098,x03099,x03100,x03101,x03102,x03103,x03104,x03105,x03106,x03107,x03108,x03109,x03110,x03111,x03112,x03113,x03114,x03115,x03116,x03117,x03118,x03119,x03120,x03121,x03122,x03123,x03124,x03125,x03126,x03127,x03128,x03129,x03130,x03131,x03132,x03133,x03134,x03135,x03136,x03137,x03138,x03139,x03140,x03141,x03142,x03143,x03144,x03145,x03146,x03147,x03148,x03149,x03150,x03151,x03152,x03153,x03154,x03155,x03156,x03157,x03158,x03159,x03160,x03161,x03162,x03163,x03164,x03165,x03166,x03167,x03168,x03169,x03170,x03171,x03172,x03173,x03174,x03175,x03176,x03177,x03178,x03179,x03180,x03181,x03182,x03183,x03184,x03185,x03186,x03187,x03188,x03189,x03190,x03191,x03192,x03193,x03194,x03195,x03196,x03197,x03198,x03199,x03200,x03201,x03202,x03203,x03204,x03205,x03206,x03207,x03208,x03209,x03210,x03211,x03212,x03213,x03214,x03215,x03216,x03217,x03218,x03219,x03220,x03221,x03222,x03223,x03224,x03225,x03226,x03227,x03228,x03229,x03230,x03231,x03232,x03233,x03234,x03235,x03236,x03237,x03238,x03239,x03240,x03241,x03242,x03243,x03244,x03245,x03246,x03247,x03248,x03249,x03250,x03251,x03252,x03253,x03254,x03255,x03256,x03257,x03258,x03259,x03260,x03261,x03262,x03263,x03264,x03265,x03266,x03267,x03268,x03269,x03270,x03271,x03272,x03273,x03274,x03275,x03276,x03277,x03278,x03279,x03280,x03281,x03282,x03283,x03284,x03285,x03286,x03287,x03288,x03289,x03290,x03291,x03292,x03293,x03294,x03295,x03296,x03297,x03298,x03299,x03300,x03301,x03302,x03303,x03304,x03305,x03306,x03307,x03308,x03309,x03310,x03311,x03312,x03313,x03314,x03315,x03316,x03317,x03318,x03319,x03320,x03321,x03322,x03323,x03324,x03325,x03326,x03327,x03328,x03329,x03330,x03331,x03332,x03333,x03334,x03335,x03336,x03337,x03338,x03339,x03340,x03341,x03342,x03343,x03344,x03345,x03346,x03347,x03348,x03349,x03350,x03351,x03352,x03353,x03354,x03355,x03356,x03357,x03358,x03359,x03360,x03361,x03362,x03363,x03364,x03365,x03366,x03367,x03368,x03369,x03370,x03371,x03372,x03373,x03374,x03375,x03376,x03377,x03378,x03379,x03380,x03381,x03382,x03383,x03384,x03385,x03386,x03387,x03388,x03389,x03390,x03391,x03392,x03393,x03394,x03395,x03396,x03397,x03398,x03399,x03400,x03401,x03402,x03403,x03404,x03405,x03406,x03407,x03408,x03409,x03410,x03411,x03412,x03413,x03414,x03415,x03416,x03417,x03418,x03419,x03420,x03421,x03422,x03423,x03424,x03425,x03426,x03427,x03428,x03429,x03430,x03431,x03432,x03433,x03434,x03435,x03436,x03437,x03438,x03439,x03440,x03441,x03442,x03443,x03444,x03445,x03446,x03447,x03448,x03449,x03450,x03451,x03452,x03453,x03454,x03455,x03456,x03457,x03458,x03459,x03460,x03461,x03462,x03463,x03464,x03465,x03466,x03467,x03468,x03469,x03470,x03471,x03472,x03473,x03474,x03475,x03476,x03477,x03478,x03479,x03480,x03481,x03482,x03483,x03484,x03485,x03486,x03487,x03488,x03489,x03490,x03491,x03492,x03493,x03494,x03495,x03496,x03497,x03498,x03499,x03500,x03501,x03502,x03503,x03504,x03505,x03506,x03507,x03508,x03509,x03510,x03511,x03512,x03513,x03514,x03515,x03516,x03517,x03518,x03519,x03520,x03521,x03522,x03523,x03524,x03525,x03526,x03527,x03528,x03529,x03530,x03531,x03532,x03533,x03534,x03535,x03536,x03537,x03538,x03539,x03540,x03541,x03542,x03543,x03544,x03545,x03546,x03547,x03548,x03549,x03550,x03551,x03552,x03553,x03554,x03555,x03556,x03557,x03558,x03559,x03560,x03561,x03562,x03563,x03564,x03565,x03566,x03567,x03568,x03569,x03570,x03571,x03572,x03573,x03574,x03575,x03576,x03577,x03578,x03579,x03580,x03581,x03582,x03583,x03584,x03585,x03586,x03587,x03588,x03589,x03590,x03591,x03592,x03593,x03594,x03595,x03596,x03597,x03598,x03599,x03600,x03601,x03602,x03603,x03604,x03605,x03606,x03607,x03608,x03609,x03610,x03611,x03612,x03613,x03614,x03615,x03616,x03617,x03618,x03619,x03620,x03621,x03622,x03623,x03624,x03625,x03626,x03627,x03628,x03629,x03630,x03631,x03632,x03633,x03634,x03635,x03636,x03637,x03638,x03639,x03640,x03641,x03642,x03643,x03644,x03645,x03646,x03647,x03648,x03649,x03650,x03651,x03652,x03653,x03654,x03655,x03656,x03657,x03658,x03659,x03660,x03661,x03662,x03663,x03664,x03665,x03666,x03667,x03668,x03669,x03670,x03671,x03672,x03673,x03674,x03675,x03676,x03677,x03678,x03679,x03680,x03681,x03682,x03683,x03684,x03685,x03686,x03687,x03688,x03689,x03690,x03691,x03692,x03693,x03694,x03695,x03696,x03697,x03698,x03699,x03700,x03701,x03702,x03703,x03704,x03705,x03706,x03707,x03708,x03709,x03710,x03711,x03712,x03713,x03714,x03715,x03716,x03717,x03718,x03719,x03720,x03721,x03722,x03723,x03724,x03725,x03726,x03727,x03728,x03729,x03730,x03731,x03732,x03733,x03734,x03735,x03736,x03737,x03738,x03739,x03740,x03741,x03742,x03743,x03744,x03745,x03746,x03747,x03748,x03749,x03750,x03751,x03752,x03753,x03754,x03755,x03756,x03757,x03758,x03759,x03760,x03761,x03762,x03763,x03764,x03765,x03766,x03767,x03768,x03769,x03770,x03771,x03772,x03773,x03774,x03775,x03776,x03777,x03778,x03779,x03780,x03781,x03782,x03783,x03784,x03785,x03786,x03787,x03788,x03789,x03790,x03791,x03792,x03793,x03794,x03795,x03796,x03797,x03798,x03799,x03800,x03801,x03802,x03803,x03804,x03805,x03806,x03807,x03808,x03809,x03810,x03811,x03812,x03813,x03814,x03815,x03816,x03817,x03818,x03819,x03820,x03821,x03822,x03823,x03824,x03825,x03826,x03827,x03828,x03829,x03830,x03831,x03832,x03833,x03834,x03835,x03836,x03837,x03838,x03839,x03840,x03841,x03842,x03843,x03844,x03845,x03846,x03847,x03848,x03849,x03850,x03851,x03852,x03853,x03854,x03855,x03856,x03857,x03858,x03859,x03860,x03861,x03862,x03863,x03864,x03865,x03866,x03867,x03868,x03869,x03870,x03871,x03872,x03873,x03874,x03875,x03876,x03877,x03878,x03879,x03880,x03881,x03882,x03883,x03884,x03885,x03886,x03887,x03888,x03889,x03890,x03891,x03892,x03893,x03894,x03895,x03896,x03897,x03898,x03899,x03900,x03901,x03902,x03903,x03904,x03905,x03906,x03907,x03908,x03909,x03910,x03911,x03912,x03913,x03914,x03915,x03916,x03917,x03918,x03919,x03920,x03921,x03922,x03923,x03924,x03925,x03926,x03927,x03928,x03929,x03930,x03931,x03932,x03933,x03934,x03935,x03936,x03937,x03938,x03939,x03940,x03941,x03942,x03943,x03944,x03945,x03946,x03947,x03948,x03949,x03950,x03951,x03952,x03953,x03954,x03955,x03956,x03957,x03958,x03959,x03960,x03961,x03962,x03963,x03964,x03965,x03966,x03967,x03968,x03969,x03970,x03971,x03972,x03973,x03974,x03975,x03976,x03977,x03978,x03979,x03980,x03981,x03982,x03983,x03984,x03985,x03986,x03987,x03988,x03989,x03990,x03991,x03992,x03993,x03994,x03995,x03996,x03997,x03998,x03999,x04000,x04001,x04002,x04003,x04004,x04005,x04006,x04007,x04008,x04009,x04010,x04011,x04012,x04013,x04014,x04015,x04016,x04017,x04018,x04019,x04020,x04021,x04022,x04023,x04024,x04025,x04026,x04027,x04028,x04029,x04030,x04031,x04032,x04033,x04034,x04035,x04036,x04037,x04038,x04039,x04040,x04041,x04042,x04043,x04044,x04045,x04046,x04047,x04048,x04049,x04050,x04051,x04052,x04053,x04054,x04055,x04056,x04057,x04058,x04059,x04060,x04061,x04062,x04063,x04064,x04065,x04066,x04067,x04068,x04069,x04070,x04071,x04072,x04073,x04074,x04075,x04076,x04077,x04078,x04079,x04080,x04081,x04082,x04083,x04084,x04085,x04086,x04087,x04088,x04089,x04090,x04091,x04092,x04093,x04094,x04095,x04096,x04097,x04098,x04099,x04100,x04101,x04102,x04103,x04104,x04105,x04106,x04107,x04108,x04109,x04110,x04111,x04112,x04113,x04114,x04115,x04116,x04117,x04118,x04119,x04120,x04121,x04122,x04123,x04124,x04125,x04126,x04127,x04128,x04129,x04130,x04131,x04132,x04133,x04134,x04135,x04136,x04137,x04138,x04139,x04140,x04141,x04142,x04143,x04144,x04145,x04146,x04147,x04148,x04149,x04150,x04151,x04152,x04153,x04154,x04155,x04156,x04157,x04158,x04159,x04160,x04161,x04162,x04163,x04164,x04165,x04166,x04167,x04168,x04169,x04170,x04171,x04172,x04173,x04174,x04175,x04176,x04177,x04178,x04179,x04180,x04181,x04182,x04183,x04184,x04185,x04186,x04187,x04188,x04189,x04190,x04191,x04192,x04193,x04194,x04195,x04196,x04197,x04198,x04199,x04200,x04201,x04202,x04203,x04204,x04205,x04206,x04207,x04208,x04209,x04210,x04211,x04212,x04213,x04214,x04215,x04216,x04217,x04218,x04219,x04220,x04221,x04222,x04223,x04224,x04225,x04226,x04227,x04228,x04229,x04230,x04231,x04232,x04233,x04234,x04235,x04236,x04237,x04238,x04239,x04240,x04241,x04242,x04243,x04244,x04245,x04246,x04247,x04248,x04249,x04250,x04251,x04252,x04253,x04254,x04255,x04256,x04257,x04258,x04259,x04260,x04261,x04262,x04263,x04264,x04265,x04266,x04267,x04268,x04269,x04270,x04271,x04272,x04273,x04274,x04275,x04276,x04277,x04278,x04279,x04280,x04281,x04282,x04283,x04284,x04285,x04286,x04287,x04288,x04289,x04290,x04291,x04292,x04293,x04294,x04295,x04296,x04297,x04298,x04299,x04300,x04301,x04302,x04303,x04304,x04305,x04306,x04307,x04308,x04309,x04310,x04311,x04312,x04313,x04314,x04315,x04316,x04317,x04318,x04319,x04320,x04321,x04322,x04323,x04324,x04325,x04326,x04327,x04328,x04329,x04330,x04331,x04332,x04333,x04334,x04335,x04336,x04337,x04338,x04339,x04340,x04341,x04342,x04343,x04344,x04345,x04346,x04347,x04348,x04349,x04350,x04351,x04352,x04353,x04354,x04355,x04356,x04357,x04358,x04359,x04360,x04361,x04362,x04363,x04364,x04365,x04366,x04367,x04368,x04369,x04370,x04371,x04372,x04373,x04374,x04375,x04376,x04377,x04378,x04379,x04380,x04381,x04382,x04383,x04384,x04385,x04386,x04387,x04388,x04389,x04390,x04391,x04392,x04393,x04394,x04395,x04396,x04397,x04398,x04399,x04400,x04401,x04402,x04403,x04404,x04405,x04406,x04407,x04408,x04409,x04410,x04411,x04412,x04413,x04414,x04415,x04416,x04417,x04418,x04419,x04420,x04421,x04422,x04423,x04424,x04425,x04426,x04427,x04428,x04429,x04430,x04431,x04432,x04433,x04434,x04435,x04436,x04437,x04438,x04439,x04440,x04441,x04442,x04443,x04444,x04445,x04446,x04447,x04448,x04449,x04450,x04451,x04452,x04453,x04454,x04455,x04456,x04457,x04458,x04459,x04460,x04461,x04462,x04463,x04464,x04465,x04466,x04467,x04468,x04469,x04470,x04471,x04472,x04473,x04474,x04475,x04476,x04477,x04478,x04479,x04480,x04481,x04482,x04483,x04484,x04485,x04486,x04487,x04488,x04489,x04490,x04491,x04492,x04493,x04494,x04495,x04496,x04497,x04498,x04499,x04500,x04501,x04502,x04503,x04504,x04505
a,1.0000,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
b,2.0000,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
c,3.0000,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(3, parser.Rows);
        Assert.AreEqual(4507, parser.Columns);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseSpaceSeparatedWithoutCommas() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"0 0 0 3
0 0 0 0
0 0 0 0
0 0 0 0
0 0 0 0
0 0 0 0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseSpaceSeparatedWithoutCommasWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"x01 x02 x03 x04
0 0 0 3
0 0 0 0
0 0 0 0
0 0 0 0
0 0 0 0
0 0 0 0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName, parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }


    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseWithColumnTypeConversionDE() {
      // If first entry of a column can be parsed as a double we assume all values are doubles.
      // However, if any of the following entries cannot be parsed as a double we convert the whole column to a string column.
      // Special care needs to be taken with missing values, NaN (n.def.) and infinity values.
      // We only support DE-DE and InvariantCulture number formats
      string tempFileName = Path.GetTempFileName();
      var deCultureInfo = CultureInfo.GetCultureInfo("DE-DE");
      WriteToFile(tempFileName,
      "str\tdbl\tdbl\tdbl" + Environment.NewLine +
      "1,3\t1,3\t0\t3" + Environment.NewLine +
      "1,3\t\t0\t0" + Environment.NewLine +
      "s\t" + double.NaN.ToString(deCultureInfo) + "\t0\t0" + Environment.NewLine + // double.NaN might have a different string representation on different systems (even when using the same CultureInfo)
      "s\t" + double.PositiveInfinity.ToString(deCultureInfo) + "\t0\t0" + Environment.NewLine +
      "s\t" + double.NegativeInfinity.ToString(deCultureInfo) + "\t0\t0" + Environment.NewLine +
      "s\t0\t0\t0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName,
          deCultureInfo.NumberFormat,
          deCultureInfo.DateTimeFormat,
          '\t',
          true);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.IsTrue(parser.Values[0] is List<string>);
        Assert.IsTrue(parser.Values[1] is List<double>);
        Assert.IsTrue(parser.Values[2] is List<double>);
        Assert.IsTrue(parser.Values[3] is List<double>);
        Assert.IsTrue(double.IsNaN((double)parser.Values[1][1])); // missing value
        Assert.IsTrue(double.IsNaN((double)parser.Values[1][2]));
        Assert.IsTrue(double.IsPositiveInfinity((double)parser.Values[1][3])); // NOTE: in DE-DE NumberFormat just "unendlich" is not allowed (compare with InvariantCulture)
        Assert.IsTrue(double.IsNegativeInfinity((double)parser.Values[1][4]));
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseWithColumnTypeConversionInvariant() {
      // see ParseWithColumnTypeConversionDE above
      // same routine only using invariant culture
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"str,dbl,dbl,dbl
1.3,1.3,0,3
1.3,,0,0
s,NaN,0,0
s,Infinity,0,0
s,-Infinity,0,0
s,0,0,0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName,
          CultureInfo.InvariantCulture.NumberFormat,
          CultureInfo.InvariantCulture.DateTimeFormat,
          ',',
          parser.AreColumnNamesInFirstLine(tempFileName));
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.IsTrue(parser.Values[0] is List<string>);
        Assert.IsTrue(parser.Values[1] is List<double>);
        Assert.IsTrue(parser.Values[2] is List<double>);
        Assert.IsTrue(parser.Values[3] is List<double>);
        Assert.IsTrue(double.IsNaN((double)parser.Values[1][1])); // missing value
        Assert.IsTrue(double.IsNaN((double)parser.Values[1][2]));
        Assert.IsTrue(double.IsPositiveInfinity((double)parser.Values[1][3])); // NOTE: in InvariantCulture +Infinity is not allowed (compare with DE-DE)
        Assert.IsTrue(double.IsNegativeInfinity((double)parser.Values[1][4]));
      }
      finally {
        File.Delete(tempFileName);
      }
    }


    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseWithTypeConversion() {
      // the parser tries to determine the column type (double, datetime, string) by looking at the values in the first few rows
      // if the values are of a different type then the type of the column is converted
      {
        // case 1 
        // default for values is double and therefore a column with all missing values should be List<double> and contain NaN
        var tmpFileName = Path.GetTempFileName();
        WriteToFile(tmpFileName,
          @"stringCol,note
,missing val
3.14,double
");

        TableFileParser parser = new TableFileParser();
        try {
          parser.Parse(tmpFileName,
            CultureInfo.InvariantCulture.NumberFormat,
            CultureInfo.InvariantCulture.DateTimeFormat,
            separator: ',', columnNamesInFirstLine: true);
          Assert.IsTrue(parser.Values[0] is List<double>);
          Assert.AreEqual(double.NaN, parser.Values[0][0]);
          Assert.AreEqual(3.14, parser.Values[0][1]);
        }
        finally {
          File.Delete(tmpFileName);
        }

      }

      {
        // case 2
        // 'The first missing values are replaced with double.NaN while the last ones with string.Empty.'

        var tmpFileName = Path.GetTempFileName();
        WriteToFile(tmpFileName,
          @"stringCol,note
,missing val
3.14,
,missing val
str,a string --> column is converted to List<string>
,missing val
");

        TableFileParser parser = new TableFileParser();
        try {
          parser.Parse(tmpFileName,
            CultureInfo.InvariantCulture.NumberFormat,
            CultureInfo.InvariantCulture.DateTimeFormat,
            separator: ',', columnNamesInFirstLine: true);
          Assert.IsTrue(parser.Values[0] is List<string>);
          Assert.AreEqual(string.Empty, parser.Values[0][0]);
          Assert.AreEqual("3.14", parser.Values[0][1]);
          Assert.AreEqual(string.Empty, parser.Values[0][2]);
          Assert.AreEqual("str", parser.Values[0][3]);
          Assert.AreEqual(string.Empty, parser.Values[0][4]);
        }
        finally {
          File.Delete(tmpFileName);
        }
      }

      {
        // case 3
        // DateTime conversion to strings
        var tmpFileName = Path.GetTempFileName();
        WriteToFile(tmpFileName,
          @"stringCol,note
,missing val
3.1.2016,
,missing val
str,a string --> column is converted to List<string>
,missing val
");

        TableFileParser parser = new TableFileParser();
        try {
          parser.Parse(tmpFileName,
            CultureInfo.InvariantCulture.NumberFormat,
            CultureInfo.InvariantCulture.DateTimeFormat,
            separator: ',', columnNamesInFirstLine: true);
          Assert.IsTrue(parser.Values[0] is List<string>);
          Assert.AreEqual(string.Empty, parser.Values[0][0]);
          Assert.AreEqual("3.1.2016", parser.Values[0][1]);
          Assert.AreEqual(string.Empty, parser.Values[0][2]);
          Assert.AreEqual("str", parser.Values[0][3]);
          Assert.AreEqual(string.Empty, parser.Values[0][4]);
        }
        finally {
          File.Delete(tmpFileName);
        }
      }
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void ParseDateTime() {
      {
        // case 1 dates and datetimes should be parsed as datetime column
        var tmpFileName = Path.GetTempFileName();
        WriteToFile(tmpFileName,
          @"stringCol,note
19.6.2016,date
19.6.2016 8:15,datetime 
");

        TableFileParser parser = new TableFileParser();
        try {
          parser.Parse(tmpFileName,
            CultureInfo.GetCultureInfo("de-de").NumberFormat,
            CultureInfo.GetCultureInfo("de-de").DateTimeFormat,
            separator: ',', columnNamesInFirstLine: true);
          Assert.IsTrue(parser.Values[0] is List<DateTime>);
          Assert.AreEqual(new DateTime(2016, 6, 19), parser.Values[0][0]);
          Assert.AreEqual(new DateTime(2016, 6, 19, 8, 15, 0), parser.Values[0][1]);

          WriteToFile(tmpFileName,
            @"stringCol,note
2016/6/19,date
2016/6/19 8:15,datetime 
");

          parser.Parse(tmpFileName,
            CultureInfo.InvariantCulture.NumberFormat,
            CultureInfo.InvariantCulture.DateTimeFormat,
            separator: ',', columnNamesInFirstLine: true);
          Assert.IsTrue(parser.Values[0] is List<DateTime>);
          Assert.AreEqual(new DateTime(2016, 6, 19), parser.Values[0][0]);
          Assert.AreEqual(new DateTime(2016, 6, 19, 8, 15, 0), parser.Values[0][1]);
        }

        finally {
          File.Delete(tmpFileName);
        }
      }

      {
        // case 2 never parse time values as datetimes
        var tmpFileName = Path.GetTempFileName();
        WriteToFile(tmpFileName,
          @"stringCol,note
8:15,time value
9:40,time value
");

        TableFileParser parser = new TableFileParser();
        try {
          parser.Parse(tmpFileName,
            CultureInfo.InvariantCulture.NumberFormat,
            CultureInfo.InvariantCulture.DateTimeFormat,
            separator: ',', columnNamesInFirstLine: true);
          Assert.IsTrue(parser.Values[0] is List<string>); // time values should be parsed as strings
          Assert.AreEqual("8:15", parser.Values[0][0]);
          Assert.AreEqual("9:40", parser.Values[0][1]);
        }
        finally {
          File.Delete(tmpFileName);
        }
      }
    }



    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void CheckTypeConversionAndLongFiles() {
      {
        // case 1 incorrect input after 500 rows should lead to exceptions
        var tmpFileName = Path.GetTempFileName();
        // create input data
        var sb = new StringBuilder();
        sb.AppendLine("col1,col2");
        for (int r = 0; r < 2000; r++) {
          sb.AppendLine("3.15, 3.15");
        }
        // add a row with only one input value
        sb.AppendLine("3.15");

        WriteToFile(tmpFileName, sb.ToString());

        TableFileParser parser = new TableFileParser();
        try {
          parser.Parse(tmpFileName,
            CultureInfo.InvariantCulture.NumberFormat,
            CultureInfo.InvariantCulture.DateTimeFormat,
            separator: ',', columnNamesInFirstLine: true);
          // Parse should fail with an exception
          Assert.Fail("expected exception TableFileParser.DataFormatException");
        }
        catch (IOException) {
          // ignore the expected exception
        }

        finally {
          File.Delete(tmpFileName);
        }
      }
      {
        // case 2
        var tmpFileName = Path.GetTempFileName();
        // create input data
        var sb = new StringBuilder();
        sb.AppendLine("doubleCol,stringCol");
        for (int r = 0; r < 2000; r++) {
          sb.AppendLine("3.15, 3.15");
        }
        // add a row with a string value --> the column should be converted to string
        sb.AppendLine("3.15,str");

        WriteToFile(tmpFileName, sb.ToString());

        TableFileParser parser = new TableFileParser();
        try {
          parser.Parse(tmpFileName,
            CultureInfo.InvariantCulture.NumberFormat,
            CultureInfo.InvariantCulture.DateTimeFormat,
            separator: ',', columnNamesInFirstLine: true);
          Assert.IsTrue(parser.Values[0] is List<double>);
          Assert.IsTrue(parser.Values[1] is List<string>);
          Assert.AreEqual(parser.Values[1][0], "3.15");
          Assert.AreEqual(parser.Values[1][2000], "str");
        }

        finally {
          File.Delete(tmpFileName);
        }
      }
    }

    private void WriteToFile(string fileName, string content) {
      using (StreamWriter writer = new StreamWriter(fileName)) {
        writer.Write(content);
      }
    }
  }
}
