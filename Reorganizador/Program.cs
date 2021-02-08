using MetadataExtractor;
using System;
using System.IO;
using System.Linq;
using Directory = System.IO.Directory;

namespace Reorganizador
{
    class Program
    {
        static void Reorganiza(string CrearCarpeta, string item)
        {
            if (!System.IO.Directory.Exists(CrearCarpeta))
            {
                System.IO.Directory.CreateDirectory(CrearCarpeta);
                System.IO.File.Move(item, CrearCarpeta + "/" + item);
            }
            else
            {
                if (!System.IO.File.Exists(CrearCarpeta + "/" + item))
                {
                    System.IO.File.Move(item, CrearCarpeta + "/" + item);
                }
                else
                {
                    Console.WriteLine("Archivo ya existe, saltado...");
                }//if
            }//if
        }
        static void NoHayTag(FileInfo fi, string item)
        {
            //No encontró ninguna etiqueta exif, habría que proceder por fecha de modificación.
            Console.WriteLine("Tag de fecha no encontrado, tomando fecha de modificación");
            string CrearCarpeta = fi.LastWriteTime.ToString("yyyy_MM_dd");
            Reorganiza(CrearCarpeta, item);
        }
        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles(".");
            foreach (var item in files)
            {
                FileInfo fi = new FileInfo(item);
                if (fi.Extension.ToLower().Equals(".cr2") || fi.Extension.ToLower().Equals(".nef")|| fi.Extension.ToLower().Equals(".jpg"))
                {
                    Console.WriteLine("Procesando: " + item.ToString());
                    var directories = ImageMetadataReader.ReadMetadata(fi.Name);
                    var infoExif = directories.Where(p => p.Name.Equals("Exif IFD0")).ToList();
                    if (infoExif != null && infoExif.Count > 0)
                    {
                        for (int i = 0; i < infoExif.Count; i++)
                        {
                            bool blTagEncontrado = false;
                            for (int j = 0; j < infoExif[i].Tags.Count; j++)
                            {
                                if (infoExif[i].Tags[j].Name.Equals("Date/Time"))
                                {
                                    blTagEncontrado = true;
                                    string strFecha = infoExif[i].Tags[j].Description;
                                    if (strFecha != null && strFecha.Length > 0)
                                    {
                                        strFecha = strFecha.Replace(":", "_");
                                        string[] yyyyMMdd = strFecha.Split(" ");
                                        string CrearCarpeta = yyyyMMdd[0];
                                        Reorganiza(CrearCarpeta, item);
                                    }
                                    else
                                    {
                                        NoHayTag(fi, item);
                                    }//if
                                    break;
                                }//if
                            }//for
                            if (!blTagEncontrado)
                            {
                                NoHayTag(fi, item);
                                break;
                            }//if
                        }//for
                    }
                    else
                    {
                        NoHayTag(fi, item);
                    }//if
                }//if
            }//foreach
        }
    }
}
