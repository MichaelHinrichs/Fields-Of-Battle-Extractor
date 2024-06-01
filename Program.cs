//Written for Fields of Battle. https://store.steampowered.com/app/370800
namespace Fields_Of_Battle_Extractor
{
    internal class Program
    {
        static BinaryReader br;
        static void Main(string[] args)
        {
            br = new(File.OpenRead(args[0]));
            if (new string(br.ReadChars(4)) != "PAK ")
                throw new Exception("Not a Fields of Battle pak file.");

            int dataStart = br.ReadInt32();
            br.BaseStream.Position = 18;
            int fileCount = br.ReadInt32();
            br.BaseStream.Position = 22;

            List<Subfile> subfiles = new();

            for (int i = 0; i < fileCount; i++)
                subfiles.Add(ReadSub());

            string path = Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]);
            Directory.CreateDirectory(path);

            foreach (Subfile file in subfiles)
            {
                br.BaseStream.Position = file.start;
                BinaryWriter bw = new(File.Create(path + "//" + file.name));
                bw.Write(br.ReadBytes(file.size));
            }
        }

        struct Subfile
        {
            public string name;
            public int start;
            public int size;
            public byte isCompressed;
        }

        static Subfile ReadSub()
        {
            br.ReadInt16();//usually 4
            string name = new string(br.ReadChars(64)).Trim('\0');
            br.ReadInt16();
            Subfile subfile = new Subfile()
            {
                name = name,
                start = br.ReadInt32(),
                size = br.ReadInt32()
            };
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            subfile.isCompressed = br.ReadByte();
            return subfile;
        }
    }
}
