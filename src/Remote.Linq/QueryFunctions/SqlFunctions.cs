﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;

namespace Remote.Linq.QueryFunctions
{
    public static class SqlFunctions
    {
        // TODO: include this functions in expression translation (from/to System.Data.Objects.SqlClient.SqlFunctions) --> needs extra flag ??? 
        public static string StringConvert(decimal? number) { return null; }
        public static string StringConvert(double? number) { return null; }
        //public static string StringConvert(decimal? number, int? length) { return null; }
        //public static string StringConvert(double? number, int? length) { return null; }
        //public static string StringConvert(decimal? number, int? length, int? decimalArg) { return null; }
        //public static string StringConvert(double? number, int? length, int? decimalArg) { return null; }
    }
}