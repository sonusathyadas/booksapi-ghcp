using System;
using System.ComponentModel.DataAnnotations;

namespace BookApi.Models;

// create a customer model class with the following properties
// id = int - primary key , auto increment
// name = string, required
// address = string, required
// phone = string, required
// email = string, required
// add data annotations to the properties

public class Customer
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    public string Phone { get; set; }
    [Required]
    public string Email { get; set; }
}
