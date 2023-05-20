using mediumish.Models;
using System.Collections.Generic;

namespace mediumish.ViewsModel
{
	public class AuthorVM
	{
		public Author Author { get; set; }
		public List<Blog> Blogs { get; set; }
	}
}
