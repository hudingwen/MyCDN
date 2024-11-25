﻿using SqlSugar;
using System;


public class RootEntityTkey<Tkey> where Tkey : IEquatable<Tkey>
{
    /// <summary>
    /// 主键ID
    /// 泛型主键Tkey
    /// </summary>
    [SugarColumn(IsNullable = false, IsPrimaryKey = true)]
    public Tkey Id { get; set; }
}
 