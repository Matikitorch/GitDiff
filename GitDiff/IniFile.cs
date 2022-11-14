﻿using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace GitDiff
{
    public class IniFile
    {
        private string Path;
        private string EXE = Assembly.GetExecutingAssembly().GetName().Name ?? throw new ArgumentNullException();

        public IniFile(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName;
        }

        public string Read(string Key, string Section = null, string Default = "")
        {
            var RetVal = new StringBuilder();
            GetString(Section ?? EXE, Key, Default, RetVal, Path);
            return RetVal.ToString().Trim();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WriteString(Section ?? EXE, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? EXE);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? EXE);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }

        private void GetString(string Section, string Key, string Default, StringBuilder RetVal, string FilePath)
        {
            string line;

            using (StreamReader sr = new StreamReader(FilePath))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.Equals(line, "[" + Section + "]"))
                    {
                        do
                        {
                            line = sr.ReadLine();
                            if ((line is null) || (line.StartsWith('['))) break;

                            if (line.StartsWith(Key))
                            {
                                RetVal.Append(line.Substring(line.IndexOf('=') + 1));
                                return;
                            }

                        } while (true);

                        if (line is null) break;
                    }
                }

                sr.Close();
            }

            if (line is null) RetVal.Append(Default);
        }

        private void WriteString(string Section, string Key, string Value, string FilePath)
        {
            throw new NotImplementedException();
        }
    }
}