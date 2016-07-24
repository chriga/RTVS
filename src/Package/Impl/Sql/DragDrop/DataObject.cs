﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Common.Core;
using Microsoft.Languages.Editor.DragDrop;

namespace Microsoft.VisualStudio.R.Package.Sql.DragDrop {
    public static class DataObject {
        public static string GetPlainText(this IDataObject dataObject) {
            var flags = dataObject.GetFlags();
            if ((flags & DataObjectFlags.ProjectItems) != 0) {
                var item = dataObject.GetProjectItems().FirstOrDefault();
                if (item != null && Path.GetExtension(item.FileName).EqualsIgnoreCase(".r")) {
                    return GetFileContent(item.FileName).Replace("'", "''");
                }
            }
            return string.Empty;
        }

        private static string GetFileContent(string file) {
            try {
                using (var sr = new StreamReader(file)) {
                    return sr.ReadToEnd().Trim();
                }
            } catch (IOException) { } catch (UnauthorizedAccessException) { }
            return string.Empty;
        }
    }
}
