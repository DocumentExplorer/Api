using System.Collections.Generic;
using DocumentExplorer.Core.Domain;

namespace DocumentExplorer.Infrastructure.Extensions
{
    public static class FileExtensions
    {
        public static IEnumerable<File> Clone(this IEnumerable<File> files)
        {
            var result = new List<File>();
            foreach(var source in files)
            {
                result.Add(new File(source.Id,source.Path, source.OrderId, source.FileType, source.IsRequired));
            }
            return result; 
        }
    }
}