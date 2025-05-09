﻿namespace Application.Dtos.User;

public class UserPageDto
{
    public IEnumerable<UserDto> Users { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}