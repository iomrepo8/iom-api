﻿using System;

namespace IOM.Models.ApiControllerModels
{
    public class BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime Created { get; set; }
    }
}