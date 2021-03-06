﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;

namespace DataAccess
{
    /// <summary>
    /// Represents a table of data. 
    /// This is primary an IEnumerable{Row} collection. 
    /// The table may be just read-only streaming over the rows, which is ideal for large files of millions of rows. 
    /// Or it may have loaded the entire table into memory, which can be ideal for mutation. 
    /// </summary>
    public abstract class DataTable
    {
        /// <summary>
        /// Name of columns in the table. Columns should be case-insensitive.
        /// If this is a mutable table, columns may be added, removed, or reordered.
        /// </summary>
        public abstract IEnumerable<string> ColumnNames { get; }

        /// <summary>
        /// Enumeration of rows in the table.
        /// Each row has a (possibly empty) value for each column.
        /// </summary>
        public abstract IEnumerable<Row> Rows { get; }

        private readonly static DataTableBuilder _builder = new DataTableBuilder();

        /// <summary>
        /// Provides access to extension methods for creating a table. Tables can be created in many ways, such as reading CSV files,
        /// building around .NET objects, filtering existing tables, etc. 
        /// </summary>
        public static DataTableBuilder New
        {
            get { return _builder; }
        }

        /// <summary>
        /// Return true if the table has the given column name. Comparison is case insensitive.
        /// </summary>
        /// <param name="name">name of column to look for.</param>
        /// <returns>true iff the column is present. False if name is null.</returns>
        public bool HasColumnName(string name)
        {
            if (name == null)
                return false;

            foreach (var c in this.ColumnNames)
            {
                if (string.Compare(name, c, true) == 0)
                {
                    return true;
                }
            }
            return false;
        }
                                
        /// <summary>
        /// Save the table to the given stream, using a CSV format. The first line will be the headers, and then each subsequent line will be a row.
        /// This will escape characters as needed.
        /// </summary>
        /// <param name="output">textwrite to write out to.</param>                
        public virtual void SaveToStream(TextWriter output)
        {
            using (var writer = new CsvWriter(output, this.ColumnNames))
            {
                foreach (var row in this.Rows)
                {
                    writer.WriteRow(row);
                }
            }
        }               

        /// <summary>
        /// Save the table as a CSV to the given filename
        /// </summary>
        /// <param name="outputFilename">filename on disk to save to.</param>
        public void SaveCSV(string outputFilename)
        {
            using (StreamWriter sw = new StreamWriter(outputFilename))
            {
                SaveToStream(sw);
            }
        }
    }
}