﻿namespace CaffWebApp.BLL.Dtos;

public class CiffDto
{
    public string Caption { get; set; } = default!;
    public int Width { get; set; }
    public int Height { get; set; }
    public string Tags { get; set; } = default!;
}
