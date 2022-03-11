﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.GUI;
using Rhino.DocObjects;
using Rhino.Collections;
using GH_IO;
using GH_IO.Serialization;
using System.Drawing;
using System.Reflection;
using Grasshopper;
using System.IO;

namespace Hops
{
    public static class HopsFunctionMgr
    {
        static string _rootDir = @"C:\Users\andyo\AppData\Roaming\Hops\Functions";

        public static void AddFunctionMgrControl()
        {
            Type editorType = typeof(Grasshopper.GUI.GH_DocumentEditor);
            if(editorType != null)
            {
                BindingFlags binding = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField;
                FieldInfo field = editorType.GetField("_CanvasToolbar", binding);
                if(field != null && Instances.DocumentEditor != null)
                {
                    try
                    {
                        object fieldInstance = field.GetValue(Instances.DocumentEditor);
                        ToolStrip toolstrip = fieldInstance as ToolStrip;
                        if (toolstrip == null)
                            return;
                        var menuItem = CreateMenuDropDown();
                        toolstrip.Items.Add(menuItem);
                    }
                    catch(Exception ex)
                    {

                    }
                }
            }
        }

        private static ToolStripItem CreateMenuDropDown()
        {
            ToolStripMenuItem fileMgr = new ToolStripMenuItem(null, FuncMgr24Icon(), DropDownItemClicked, "Functions");
            return fileMgr;
        }

        private static void DropDownItemClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem fileMgr = sender as ToolStripMenuItem;
            if (fileMgr != null)
            {
                fileMgr.DropDownItems.Clear();
                GenerateFunctionPathMenu(fileMgr);
            }
        }

        private static void GenerateFunctionPathMenu(ToolStripMenuItem menu)
        {
            if (!String.IsNullOrEmpty(_rootDir))
            {
                FunctionPathInfo functionPaths = new FunctionPathInfo(_rootDir, true);
                functionPaths.isRoot = true;

                SeekFunctionMenuDirs(functionPaths);
                if (functionPaths.Paths.Count != 0)
                {
                    functionPaths.BuildMenus(menu, new EventHandler(tsm_Click));
                }
            }
        }

        public static void SeekFunctionMenuDirs(FunctionPathInfo path)
        {
            if (path == null || !path.IsValid())
                return;

            string[] files = Directory.GetFiles(path.FullPath);
            foreach(string file in files)
            {
                FunctionPathInfo filePath = new FunctionPathInfo(file, false);
                path.Paths.Add(filePath);
            }

            string[] subDirs = Directory.GetDirectories(path.FullPath);
            foreach (string subDir in subDirs)
            {
                FunctionPathInfo subDirPath = new FunctionPathInfo(subDir, true);
                path.Paths.Add(subDirPath);
                SeekFunctionMenuDirs(subDirPath);
            }
        }

        static void tsm_Click(object sender, EventArgs e)
        {
            if (!(sender is ToolStripItem))
                return;
            ToolStripItem ti = sender as ToolStripItem;

            string filename = Path.GetFileNameWithoutExtension(ti.Name);
        }

        static Image _funcMgr24Icon;
        static Image _funcMgr48Icon;
        static Image FuncMgr24Icon()
        {
            if (_funcMgr24Icon == null)
            {
                var stream = typeof(HopsComponent).Assembly.GetManifestResourceStream("Hops.resources.Hops_Function_Mgr_24x24.png");
                _funcMgr24Icon = Image.FromStream(stream);
            }
            return _funcMgr24Icon;
        }
        static Image FuncMgr48Icon()
        {
            if (_funcMgr48Icon == null)
            {
                var stream = typeof(HopsComponent).Assembly.GetManifestResourceStream("Hops.resources.Hops_Function_Mgr_48x48.png");
                _funcMgr48Icon = Image.FromStream(stream);
            }
            return _funcMgr48Icon;
        }
    }
}
