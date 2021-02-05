using MetadataExtractor;
using System;
using System.IO;
using System.Linq;
using Directory = System.IO.Directory;

namespace Reorganizador
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles(".");
            foreach (var item in files)
            {
                Console.WriteLine("Procesando: " + item.ToString());
                FileInfo fi = new FileInfo(item);
                if (fi.Extension.ToLower().Equals(".cr2") || fi.Extension.ToLower().Equals(".nef")|| fi.Extension.ToLower().Equals(".jpg"))
                {
                    var directories = ImageMetadataReader.ReadMetadata(fi.Name);
                    var infoExif = directories.Where(p => p.Name.Equals("Exif IFD0")).ToList();
                    if (infoExif != null)
                    {
                        for (int i=0; i < infoExif.Count; i++)
                        {
                            bool blTagEncontrado = false;
                            for (int j=0; j < infoExif[i].Tags.Count; j++)
                            {
                                if (infoExif[i].Tags[j].Name.Equals("Date/Time"))
                                {
                                    blTagEncontrado = true;
                                    string strFecha = infoExif[i].Tags[j].Description;
                                    strFecha = strFecha.Replace(":", "_");
                                    string[] yyyyMMdd = strFecha.Split(" ");
                                    string CrearCarpeta = yyyyMMdd[0];
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
                            if (!blTagEncontrado)
                            {
                                Console.WriteLine("Tag de fecha no encontrado, tomando fecha de modificación");
                                string CrearCarpeta = fi.LastWriteTime.ToString("yyyy_MM_dd");
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
                    }//if
                }//if
            }//foreach
        }
    }
}
