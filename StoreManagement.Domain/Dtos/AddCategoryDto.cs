﻿namespace StoreManagement.Domain.Dtos;

public class AddCategoryDto
{
    public required string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
