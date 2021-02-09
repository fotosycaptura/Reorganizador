using MetadataExtractor;
using System;
using System.IO;
using System.Linq;
using Directory = System.IO.Directory;
using System.Reflection;
using CommandLine;

namespace Reorganizador
{
    class Program
    {
        public class Options
        {
            [Option('h', "help", Required = false, HelpText = "Muestra esta ayuda")]
            public bool Help { get; private set; }
        }
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
                    //Console.WriteLine("Archivo ya existe, saltado...");
                }//if
            }//if
        }
        static void NoHayTag(FileInfo fi, string item)
        {
            //No encontró ninguna etiqueta exif, habría que proceder por fecha de modificación.
            //Console.WriteLine("Tag de fecha no encontrado, tomando fecha de modificación");
            string CrearCarpeta = fi.LastWriteTime.ToString("yyyy_MM_dd");
            Reorganiza(CrearCarpeta, item);
        }
        private static void progreso(int progreso, int total = 100) //Default 100
        {
            //Dibujar la barra vacia
            Console.CursorLeft = 0;
            Console.Write("."); //inicio
            Console.CursorLeft = 31;
            Console.Write("."); //fin
            Console.CursorLeft = 1; //Colocar el cursor al inicio
            float onechunk = 30.0f / total;

            //Rellenar la parte indicada
            int position = 1;
            for (int i = 0; i < onechunk * progreso; i++)
            {
                Console.CursorLeft = position++;
                Console.Write(".");
            }

            //Pintar la otra parte
            for (int i = position; i <= 31; i++)
            {
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //Escribir el total al final
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progreso.ToString() + " de " + total.ToString() + " procesado...   ");
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Reorganizador de fotos");
            Console.WriteLine("Más información en: https://github.com/fotosycaptura/Reorganizador");
            ///Para los parámetros
            ///
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts =>
                {
                    if (opts.Help)
                    {
                        Console.WriteLine("Aplicación de consola para ayudar a reorganizar lotes de fotos del");
                        Console.WriteLine("mismo directorio o carpeta. Reorganiza el lote de fotos en carpetas");
                        Console.WriteLine("yyyy_MM_dd, según los datos exif. Creado con .NetCore 5");
                    }
                    else
                    {
                        Console.WriteLine("Reorganizando...");
                        string[] files = Directory.GetFiles(".");
                        int inicio = 1;
                        int termino = files.Count();
                        foreach (var item in files)
                        {
                            FileInfo fi = new FileInfo(item);
                            progreso(inicio, termino);
                            if (fi.Extension.ToLower().Equals(".cr2") || fi.Extension.ToLower().Equals(".nef") || fi.Extension.ToLower().Equals(".jpg"))
                            {
                                //Console.WriteLine("Procesando: " + item.ToString());
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
                            inicio++;
                        }//foreach
                        progreso(termino, termino);
                        Console.WriteLine("... Finalizado.");
                    }//if
                });
        }
        
    }
}
