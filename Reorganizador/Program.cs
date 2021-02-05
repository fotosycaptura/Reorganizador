using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using System;
using System.IO;
using System.Linq;
using Directory = System.IO.Directory;

namespace Reorganizador
{
    class Program
    {
        private static string imagePath;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            imagePath = "*.jpg";

            string[] files = Directory.GetFiles(".");
            foreach (var item in files)
            {
                Console.WriteLine(item.ToString());
                FileInfo fi = new FileInfo(item);
                if (fi.Extension.Equals(".jpg"))
                {
                    var directories = ImageMetadataReader.ReadMetadata(fi.Name);
                    var infoExif = directories.Where(p => p.Name.Equals("Exif IFD0")).ToList();
                    if (infoExif != null)
                    {
                        
                        for (int i=0; i < infoExif.Count; i++)
                        {
                            for (int j=0; j < infoExif[i].Tags.Count; j++)
                            {
                                if (infoExif[i].Tags[j].Name.Equals("Date/Time"))
                                {
                                    string strFecha = infoExif[i].Tags[j].Description;
                                    string[] valores = strFecha.Split(":");
                                    string CrearCarpeta = valores[0] + "_" + valores[1] + "_" + valores[2];
                                    if (!System.IO.Directory.Exists(CrearCarpeta))
                                    {
                                        System.IO.Directory.CreateDirectory(CrearCarpeta);
                                        System.IO.File.Move(item, CrearCarpeta + "/" + item);
                                    }
                                    else
                                    {
                                        System.IO.File.Move(item, CrearCarpeta + "/" + item);
                                    }
                                    break;
                                }//if
                            }//for
                        }//for
                    }//if
                }//if
            }//foreach
        }
    }
}
