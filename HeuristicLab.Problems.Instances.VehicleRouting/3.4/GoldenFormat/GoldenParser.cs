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
using System.IO;

namespace HeuristicLab.Problems.Instances.VehicleRouting {
  public class GoldenParser {
    private string file;
    private Stream stream;
    private string problemName;

    #region Inner Enum TSPLIBEdgeWeightType
    public enum GoldenEdgeWeightType {
      UNDEFINED,
      EUC_2D,
      GEO
    }
    #endregion

    private const int EOF = 0;
    private const int NAME = 1;
    private const int TYPE = 2;
    private const int COMMENT = 3;
    private const int DIM = 4;
    private const int WEIGHTTYPE = 5;
    private const int WEIGHTFORMAT = 11;
    private const int NODETYPE = 6;
    private const int NODESECTION = 7;
    private const int CAPACITY = 8;
    private const int DISTANCE = 12;
    private const int VEHICLES = 13;
    private const int DEPOTSECTION = 9;
    private const int DEMANDSECTION = 10;

    private string comment;
    /// <summary>
    /// Gets the comment of the parsed VRP.
    /// </summary>
    public string Comment {
      get { return comment; }
    }
    private double[,] vertices;
    /// <summary>
    /// Gets the vertices of the parsed VRP.
    /// </summary>
    public double[,] Vertices {
      get { return vertices; }
    }
    private GoldenEdgeWeightType weightType;
    /// <summary>
    /// Gets the weight format of the parsed VRP.
    /// </summary>
    public GoldenEdgeWeightType WeightType {
      get { return weightType; }
    }

    private double capacity;
    public double Capacity {
      get {
        return capacity;
      }
    }

    private int vehicles;
    public int Vehicles {
      get {
        return vehicles;
      }
    }

    private double distance;
    public double Distance {
      get {
        return distance;
      }
    }

    private double[] demands;
    public double[] Demands {
      get {
        return demands;
      }
    }

    public String ProblemName {
      get {
        return problemName;
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TSPLIBParser"/> with the given <paramref name="path"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if the input file is not a TSPLIB TSP file (*.vrp)
    /// </exception>
    /// <param name="path">The path where the VRP is stored.</param>
    public GoldenParser() {
      comment = string.Empty;
      vertices = null;
      weightType = GoldenEdgeWeightType.UNDEFINED;
      capacity = -1;
      vehicles = -1;
      distance = -1;
      demands = null;
    }

    public GoldenParser(string file)
      : this() {
      this.file = file;
    }

    public GoldenParser(Stream stream)
      : this() {
      this.stream = stream;
    }

    /// <summary>
    /// Reads the TSPLIB VRP file and parses the elements.
    /// </summary>
    /// <exception cref="InvalidDataException">Thrown if the file has an invalid format or contains invalid data.</exception>
    public void Parse() {
      StreamReader reader;
      if (stream != null) {
        reader = new StreamReader(stream);
      } else {
        reader = new StreamReader(file);
        problemName = Path.GetFileNameWithoutExtension(file);
      }

      using (reader) {
        int section = -1;
        string str = null;
        bool weightFormatIsChecked = false;

        do {
          str = reader.ReadLine();
          section = GetSection(str);

          if (section != -1) {
            switch (section) {
              case NAME:
                ReadName(str);
                break;
              case TYPE:
                CheckType(str);
                break;
              case COMMENT:
                ReadComment(str);
                break;
              case DIM:
                InitArrays(str);
                break;
              case WEIGHTTYPE:
                ReadWeightType(str);
                break;
              case WEIGHTFORMAT:
                ReadWeightFormat(str);
                weightFormatIsChecked = true;
                break;
              case NODETYPE:
                CheckNodeType(str);
                break;
              case NODESECTION:
                ReadVertices(reader);
                break;
              case CAPACITY:
                ReadCapacity(str);
                break;
              case VEHICLES:
                ReadVehicles(str);
                break;
              case DISTANCE:
                ReadDistance(str);
                break;
              case DEPOTSECTION:
                ReadDepot(reader);
                break;
              case DEMANDSECTION:
                ReadDemands(reader);
                break;
            }
          }
        } while (!((section == EOF) || (str == null)));

        if (!weightFormatIsChecked || weightType == GoldenEdgeWeightType.UNDEFINED)
          throw new InvalidDataException("Input file does not contain format or edge weight format information.");
      }
    }

    private int GetSection(string str) {
      if (str == null)
        return EOF;

      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      if (tokens.Length == 0)
        return -1;

      string token = tokens[0].Trim();
      if (token.Equals("eof", StringComparison.OrdinalIgnoreCase))
        return EOF;
      if (token.Equals("name", StringComparison.OrdinalIgnoreCase))
        return NAME;
      if (token.Equals("type", StringComparison.OrdinalIgnoreCase))
        return TYPE;
      if (token.Equals("comment", StringComparison.OrdinalIgnoreCase))
        return COMMENT;
      if (token.Equals("dimension", StringComparison.OrdinalIgnoreCase))
        return DIM;
      if (token.Equals("edge_weight_type", StringComparison.OrdinalIgnoreCase))
        return WEIGHTTYPE;
      if (token.Equals("edge_weight_format", StringComparison.OrdinalIgnoreCase))
        return WEIGHTFORMAT;
      if (token.Equals("node_coord_type", StringComparison.OrdinalIgnoreCase))
        return NODETYPE;
      if (token.Equals("node_coord_section", StringComparison.OrdinalIgnoreCase))
        return NODESECTION;
      if (token.Equals("capacity", StringComparison.OrdinalIgnoreCase))
        return CAPACITY;
      if (token.Equals("vehicles", StringComparison.OrdinalIgnoreCase))
        return VEHICLES;
      if (token.Equals("distance", StringComparison.OrdinalIgnoreCase))
        return DISTANCE;
      if (token.Equals("depot_section", StringComparison.OrdinalIgnoreCase))
        return DEPOTSECTION;
      if (token.Equals("demand_section", StringComparison.OrdinalIgnoreCase))
        return DEMANDSECTION;

      return -1;
    }

    private void ReadName(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      problemName = tokens[tokens.Length - 1].Trim();
    }

    private void CheckType(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);

      string type = tokens[tokens.Length - 1].Trim();
      if (!type.Equals("cvrp", StringComparison.OrdinalIgnoreCase))
        throw new InvalidDataException("Input file format is not \"CVRP\"");
    }

    private void ReadComment(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      comment = tokens[tokens.Length - 1].Trim();
    }

    private void InitArrays(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      string dimension = tokens[tokens.Length - 1].Trim();

      int dim = Int32.Parse(dimension);
      vertices = new double[dim, 2];
      demands = new double[dim];
    }

    private void ReadWeightType(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      string type = tokens[tokens.Length - 1].Trim();

      if (type.Equals("euc_2d", StringComparison.OrdinalIgnoreCase))
        weightType = GoldenEdgeWeightType.EUC_2D;
      else if (type.Equals("geo", StringComparison.OrdinalIgnoreCase))
        weightType = GoldenEdgeWeightType.GEO;
      else if (type.Equals("function", StringComparison.OrdinalIgnoreCase)) {
        weightType = GoldenEdgeWeightType.UNDEFINED;
      } else
        throw new InvalidDataException("Input file contains an unsupported edge weight format (only \"EUC_2D\" and \"GEO\" are supported).");
    }

    private void ReadWeightFormat(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      string format = tokens[tokens.Length - 1].Trim();

      if (format.Equals("euc_2d", StringComparison.OrdinalIgnoreCase))
        weightType = GoldenEdgeWeightType.EUC_2D;
      else if (format.Equals("geo", StringComparison.OrdinalIgnoreCase))
        weightType = GoldenEdgeWeightType.GEO;
      else
        throw new InvalidDataException("Input file contains an unsupported edge weight format.");
    }

    private void CheckNodeType(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      string type = tokens[tokens.Length - 1].Trim();

      if (!type.Equals("twod_coords", StringComparison.OrdinalIgnoreCase))
        throw new InvalidDataException("Input file contains an unsupported node coordinates format (only \"TWOD_COORDS\" is supported).");
    }

    private void ReadVertices(StreamReader reader) {
      if (vertices == null)
        throw new InvalidDataException("Input file does not contain dimension information.");

      for (int i = 1; i < (vertices.Length / 2); i++) {
        string str = reader.ReadLine();
        string[] tokens = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length != 3)
          throw new InvalidDataException("Input file contains invalid node coordinates.");

        vertices[i, 0] = double.Parse(tokens[1], System.Globalization.CultureInfo.InvariantCulture);
        vertices[i, 1] = double.Parse(tokens[2], System.Globalization.CultureInfo.InvariantCulture);
      }
    }

    private void ReadCapacity(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      capacity = double.Parse(tokens[tokens.Length - 1].Trim(), System.Globalization.CultureInfo.InvariantCulture);
    }

    private void ReadVehicles(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      vehicles = int.Parse(tokens[tokens.Length - 1].Trim());
    }

    private void ReadDistance(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      distance = double.Parse(tokens[tokens.Length - 1].Trim(), System.Globalization.CultureInfo.InvariantCulture);
    }

    private void ReadDepot(StreamReader reader) {
      string[] tokens;
      do {
        string str = reader.ReadLine();
        tokens = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length == 2) {
          vertices[0, 0] = double.Parse(tokens[0], System.Globalization.CultureInfo.InvariantCulture);
          vertices[0, 1] = double.Parse(tokens[1], System.Globalization.CultureInfo.InvariantCulture);
        }
      } while (tokens.Length == 2);
    }

    private void ReadDemands(StreamReader reader) {
      if (demands == null)
        throw new InvalidDataException("Input file does not contain dimension information.");

      for (int i = 0; i < demands.Length; i++) {
        if (i != 0) {
          string str = reader.ReadLine();
          string[] tokens = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

          if (tokens.Length != 2)
            throw new InvalidDataException("Input file contains invalid demands.");

          int index = int.Parse(tokens[0]);
          double value = double.Parse(tokens[1], System.Globalization.CultureInfo.InvariantCulture);

          demands[index] = value;
        } else {
          demands[0] = 0;
        }
      }
    }
  }
}
