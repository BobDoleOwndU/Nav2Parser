using System;
using System.Collections.Generic;
using System.IO;

namespace Nav2Parser
{
    class Nav2
    {
        private struct Header
        {
            public uint version;
            public uint fileLength;
            public uint entriesOffset;
            public uint entryCount;
            public uint navSystemOffset;
            public ushort u4a;
            public ushort o5;
            public uint section2Offset;
            public uint o6;
            public Vector3 origin;
            public uint u1a; //Offset?
            public uint u1b;
            public uint manifsetOffset;
            public uint manifestLength;
            public uint u1c;
            public uint u1d;
            public ushort u1e; //Offset?
            public short u1f; //Always -1?
            public ushort u1g; //Offset?
            public ushort u1h; //Offset/Count?
            public byte n7;
            public byte section2EntryCount;
            public ushort n8;
            public Vector4Half unknown8;
            public Vector4Half unknown9;
            public uint manifestEntryCount;
        } //Header

        private struct ManifestEntry
        {
            public byte groupId;
            public byte u1b;
            public ushort u2;
            public uint payloadOffset;
            public byte entrySize;
            public ushort n4;
        } //ManifestEntry

        private struct Manifest
        {
            public ManifestEntry[] manifestEntries; //Always 3?
        } //Manifest

        private struct Entry
        {
            public ushort typeEnum;
            public ushort u1;
            public uint nextEntryRelativeOffset;
            public uint payloadRelativeOffset;
            public ushort n1;
            public ushort n2;
        } //Entry

        private struct NavWorld
        {
            public uint subsection1Offset;
            public uint subsection2Offset;
            public uint subsection4Offset;
            public uint subsection3Offset; //Not read by exe?
            public uint u1; //Not read by exe? Always 0?
            public uint u2; //Always 0?
            public uint subsection5Offset; //Not read by exe?
            public uint u3; //Not read by exe? Always 0?
            public uint subsection6Offset;
            public ushort subsection1EntryCount;
            public ushort subsection4EntryCount;
            public ushort subsection5EntryCount;
            public NavWorldSubsection1Entry[] navWorldSubsection1Entries;
            public NavWorldSubsection2Entry[] navWorldSubsection2Entries;
            public ushort[] navWorldSubsection3Entries;
            public NavWorldSubsection4Entry[] navWorldSubsection4Entries;
            public NavWorldSubsection5Entry[] navWorldSubsection5Entries;
            public short[] navWorldSubsection6Entries;
        } //NavWorld

        private struct NavWorldSubsection1Entry
        {
            public byte u1a;
            public byte u1b;
            public short u1;
            public byte u2a;
            public byte u2b;
        } //NavWorldSubsection1Entry

        private struct NavWorldSubsection2Entry
        {
            public short u1; //index
            public short u2;
            public byte u3; //count * 2
            public byte u4; //count
        } //NavWorldSubsection2Entry

        private struct NavWorldSubsection4Entry
        {
            public short u1;
            public short u2;
            public byte u3;
            public byte u4;
        } //NavWorldSubsection4Entry

        private struct NavWorldSubsection5Entry
        {
            public byte u1;
            public byte u2;
        } //NavWorldSubsection5Entry


        /****************************************************************
         * VARIABLES
         ****************************************************************/
        Header header;
        Manifest[] manifests;
        Entry[] entries;
        List<NavWorld> navWorlds;

        /****************************************************************
         * PUBLIC FUNCTIONS
         ****************************************************************/
        public void Read(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                try
                {
                    BinaryReader reader = new BinaryReader(stream);
                    header = new Header();
                    navWorlds = new List<NavWorld>(0);

                    ReadHeader(reader);
                    reader.BaseStream.Position = header.manifsetOffset;
                    ReadManifest(reader);
                    reader.BaseStream.Position = header.entriesOffset;
                    ReadEntries(reader);
                } //try
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    Console.Write(e.StackTrace);
                } //catch
                finally
                {
                    stream.Close();
                } //finally
            } //using
        } //Read

        /****************************************************************
         * PRIVATE FUNCTIONS
         ****************************************************************/
        private void ReadHeader(BinaryReader reader)
        {
            header.version = reader.ReadUInt32();
            header.fileLength = reader.ReadUInt32();
            header.entriesOffset = reader.ReadUInt32();
            header.entryCount = reader.ReadUInt32();
            header.navSystemOffset = reader.ReadUInt32();
            header.u4a = reader.ReadUInt16();
            header.o5 = reader.ReadUInt16();
            header.section2Offset = reader.ReadUInt32();
            header.o6 = reader.ReadUInt32();
            header.origin = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            header.u1a = reader.ReadUInt32();
            header.u1b = reader.ReadUInt32();
            header.manifsetOffset = reader.ReadUInt32();
            header.manifestLength = reader.ReadUInt32();
            header.u1c = reader.ReadUInt32();
            header.u1d = reader.ReadUInt32();
            header.u1e = reader.ReadUInt16();
            header.u1f = reader.ReadInt16();
            header.u1g = reader.ReadUInt16();
            header.u1h = reader.ReadUInt16();
            header.n7 = reader.ReadByte();
            header.section2EntryCount = reader.ReadByte();
            header.n8 = reader.ReadUInt16();
            header.unknown8 = new Vector4Half(Half.ToHalf(reader.ReadUInt16()), Half.ToHalf(reader.ReadUInt16()), Half.ToHalf(reader.ReadUInt16()), Half.ToHalf(reader.ReadUInt16()));
            header.unknown9 = new Vector4Half(Half.ToHalf(reader.ReadUInt16()), Half.ToHalf(reader.ReadUInt16()), Half.ToHalf(reader.ReadUInt16()), Half.ToHalf(reader.ReadUInt16()));
        } //ReadHeader

        private void ReadManifest(BinaryReader reader)
        {
            header.manifestEntryCount = reader.ReadUInt32();

            int manifestEntryCount = (int)header.manifestEntryCount;
            manifests = new Manifest[manifestEntryCount];

            for (int i = 0; i < manifestEntryCount; i++)
            {
                Manifest manifest = manifests[i];
                manifest.manifestEntries = new ManifestEntry[3];
                int manifestEntriesLength = manifest.manifestEntries.Length;

                for (int j = 0; j < manifestEntriesLength; j++)
                {
                    ManifestEntry entry = manifest.manifestEntries[j];

                    entry.groupId = reader.ReadByte();
                    entry.u1b = reader.ReadByte();
                    entry.u2 = reader.ReadUInt16();
                    entry.payloadOffset = reader.ReadUInt32();
                    entry.entrySize = reader.ReadByte();
                    entry.n4 = reader.ReadUInt16();

                    manifest.manifestEntries[j] = entry;
                } //for

                manifests[i] = manifest;
            } //for
        } //ReadManifest

        private void ReadEntries(BinaryReader reader)
        {
            int entryCount = (int)header.entryCount;
            entries = new Entry[entryCount];

            //Should be to entryCount; but will cause errors if it is in its current state.
            for (int i = 0; i < 1; i++)
            {
                Entry entry = entries[i];

                entry.typeEnum = reader.ReadUInt16();
                entry.u1 = reader.ReadUInt16();
                entry.nextEntryRelativeOffset = reader.ReadUInt32();
                entry.payloadRelativeOffset = reader.ReadUInt32();
                entry.n1 = reader.ReadUInt16();
                entry.n2 = reader.ReadUInt16();

                switch (entry.typeEnum)
                {
                    case 0:
                        ReadNavWorld(reader);
                        break;
                    default:
                        break;
                } //switch

                entries[i] = entry;
            } //for
        } //ReadEntries

        private void ReadNavWorld(BinaryReader reader)
        {
            NavWorld navWorld = new NavWorld();
            long startPosition = reader.BaseStream.Position;

            navWorld.subsection1Offset = reader.ReadUInt32();
            navWorld.subsection2Offset = reader.ReadUInt32();
            navWorld.subsection4Offset = reader.ReadUInt32();
            navWorld.subsection3Offset = reader.ReadUInt32();
            navWorld.u1 = reader.ReadUInt32();
            navWorld.u2 = reader.ReadUInt32();
            navWorld.subsection5Offset = reader.ReadUInt32();
            navWorld.u3 = reader.ReadUInt32();
            navWorld.subsection6Offset = reader.ReadUInt32();
            navWorld.subsection1EntryCount = reader.ReadUInt16();
            navWorld.subsection4EntryCount = reader.ReadUInt16();
            reader.BaseStream.Position += 6;
            navWorld.subsection5EntryCount = reader.ReadUInt16();

            int count = navWorld.subsection1EntryCount;
            navWorld.navWorldSubsection1Entries = new NavWorldSubsection1Entry[count];
            navWorld.navWorldSubsection2Entries = new NavWorldSubsection2Entry[count];

            reader.BaseStream.Position = startPosition + navWorld.subsection1Offset;

            for (int i = 0; i < count; i++)
            {
                NavWorldSubsection1Entry navWorldSubsection1Entry = navWorld.navWorldSubsection1Entries[i];

                navWorldSubsection1Entry.u1a = reader.ReadByte();
                navWorldSubsection1Entry.u1b = reader.ReadByte();
                navWorldSubsection1Entry.u1 = reader.ReadInt16();
                navWorldSubsection1Entry.u2a = reader.ReadByte();
                navWorldSubsection1Entry.u2b = reader.ReadByte();

                navWorld.navWorldSubsection1Entries[i] = navWorldSubsection1Entry;
            } //for

            reader.BaseStream.Position = startPosition + navWorld.subsection2Offset;

            for (int i = 0; i < count; i++)
            {
                NavWorldSubsection2Entry navWorldSubsection2 = navWorld.navWorldSubsection2Entries[i];

                navWorldSubsection2.u1 = reader.ReadInt16();
                navWorldSubsection2.u2 = reader.ReadInt16();
                navWorldSubsection2.u3 = reader.ReadByte();
                navWorldSubsection2.u4 = reader.ReadByte();

                navWorld.navWorldSubsection2Entries[i] = navWorldSubsection2;
            } //for

            count = navWorld.navWorldSubsection2Entries[count - 1].u1 + navWorld.navWorldSubsection2Entries[count - 1].u3 * 2 + navWorld.navWorldSubsection2Entries[count - 1].u4;
            navWorld.navWorldSubsection3Entries = new ushort[count];

            reader.BaseStream.Position = startPosition + navWorld.subsection3Offset;

            for (int i = 0; i < count; i++)
            {
                navWorld.navWorldSubsection3Entries[i] = reader.ReadUInt16();
            } //for

            count = navWorld.subsection4EntryCount;
            navWorld.navWorldSubsection4Entries = new NavWorldSubsection4Entry[count];

            reader.BaseStream.Position = startPosition + navWorld.subsection4Offset;

            for (int i = 0; i < count; i++)
            {
                NavWorldSubsection4Entry navWorldSubsection4Entry = navWorld.navWorldSubsection4Entries[i];

                navWorldSubsection4Entry.u1 = reader.ReadInt16();
                navWorldSubsection4Entry.u2 = reader.ReadInt16();
                navWorldSubsection4Entry.u3 = reader.ReadByte();
                navWorldSubsection4Entry.u4 = reader.ReadByte();

                navWorld.navWorldSubsection4Entries[i] = navWorldSubsection4Entry;
            } //for

            count = navWorld.subsection5EntryCount;
            navWorld.navWorldSubsection5Entries = new NavWorldSubsection5Entry[count];

            reader.BaseStream.Position = startPosition + navWorld.subsection5Offset;

            for (int i = 0; i < count; i++)
            {
                NavWorldSubsection5Entry navWorldSubsection5Entry = navWorld.navWorldSubsection5Entries[i];

                navWorldSubsection5Entry.u1 = reader.ReadByte();
                navWorldSubsection5Entry.u2 = reader.ReadByte();

                navWorld.navWorldSubsection5Entries[i] = navWorldSubsection5Entry;
            } //for

            count = navWorld.subsection1EntryCount;
            navWorld.navWorldSubsection6Entries = new short[count];

            reader.BaseStream.Position = startPosition + navWorld.subsection6Offset;

            for (int i = 0; i < count; i++)
            {
                navWorld.navWorldSubsection6Entries[i] = reader.ReadInt16();
            } //for

            navWorlds.Add(navWorld);
        } //ReadNavWorld

        /****************************************************************
         * DEBUG FUNCTIONS
         ****************************************************************/
        public void DisplayHeaderInfo()
        {
            Console.WriteLine($"\nFile Version: {header.version}");
            Console.WriteLine($"File Length: 0x{header.fileLength.ToString("x")}");
            Console.WriteLine($"Entries Offset: 0x{header.entriesOffset.ToString("x")}");
            Console.WriteLine($"Entry Count: 0x{header.entryCount.ToString("x")}");
            Console.WriteLine($"u4a: 0x{header.u4a.ToString("x")}");
            Console.WriteLine($"o5: 0x{header.o5.ToString("x")}");
            Console.WriteLine($"Section 2 Offset: 0x{header.section2Offset.ToString("x")}");
            Console.WriteLine($"o6: 0x{header.o6.ToString("x")}");
            Console.WriteLine($"Origin: X: {header.origin.x}, Y: {header.origin.y}, Z: {header.origin.z}");
            Console.WriteLine($"u1a: 0x{header.u1a.ToString("x")}");
            Console.WriteLine($"u1b: 0x{header.u1b.ToString("x")}");
            Console.WriteLine($"Manifest Offset: 0x{header.manifsetOffset.ToString("x")}");
            Console.WriteLine($"Manifest Length: 0x{header.manifestLength.ToString("x")}");
            Console.WriteLine($"u1c: 0x{header.u1c.ToString("x")}");
            Console.WriteLine($"u1d: 0x{header.u1d.ToString("x")}");
            Console.WriteLine($"u1e: 0x{header.u1e.ToString("x")}");
            Console.WriteLine($"u1f: 0x{header.u1f.ToString("x")}");
            Console.WriteLine($"u1g: 0x{header.u1g.ToString("x")}");
            Console.WriteLine($"u1h: 0x{header.u1h.ToString("x")}");
            Console.WriteLine($"n7: 0x{header.n7.ToString("x")}");
            Console.WriteLine($"Section 2 Entry Count: 0x{header.section2EntryCount.ToString("x")}");
            Console.WriteLine($"n8: 0x{header.n8.ToString("x")}");
            Console.WriteLine($"Unknown 8: X: {header.unknown8.x}, Y: {header.unknown8.y}, Z: {header.unknown8.z}, W: {header.unknown8.w}");
            Console.WriteLine($"Unknown 9: X: {header.unknown9.x}, Y: {header.unknown9.y}, Z: {header.unknown9.z}, W: {header.unknown9.w}");
            Console.WriteLine($"Manifest Entry Count: 0x{header.manifestEntryCount.ToString("x")}\n");
        } //DisplayHeaderInfo

        public void DisplayManifestInfo()
        {
            int manifestCount = manifests.Length;

            for (int i = 0; i < manifestCount; i++)
            {
                Manifest manifest = manifests[i];
                int entryCount = manifest.manifestEntries.Length;

                Console.WriteLine($"\nManifest: {i}");
                Console.WriteLine("================================================================");

                for(int j = 0; j < entryCount; j++)
                {
                    ManifestEntry entry = manifest.manifestEntries[j];

                    Console.WriteLine($"\nEntry: {j}");
                    Console.WriteLine("----------------------------------------------------------------");
                    Console.WriteLine($"Group Id: 0x{entry.groupId.ToString("x")}");
                    Console.WriteLine($"u1b: 0x{entry.u1b.ToString("x")}");
                    Console.WriteLine($"u2: 0x{entry.u2.ToString("x")}");
                    Console.WriteLine($"Payload Offset: 0x{entry.payloadOffset.ToString("x")}");
                    Console.WriteLine($"Entry Size: 0x{entry.entrySize.ToString("x")}");
                    Console.WriteLine($"n4: 0x{entry.n4.ToString("x")}\n");
                } //for
            } //for
        } //DisplayManifestInfo

        public void DisplayNavWorldInfo()
        {
            int navWorldCount = navWorlds.Count;

            for(int i = 0; i < navWorldCount; i++)
            {
                NavWorld navWorld = navWorlds[i];

                Console.WriteLine($"\nNavWorld: {i}");
                Console.WriteLine("================================================================");
                Console.WriteLine($"Subsection 1 Offset: 0x{navWorld.subsection1Offset.ToString("x")}");
                Console.WriteLine($"Subsection 2 Offset: 0x{navWorld.subsection2Offset.ToString("x")}");
                Console.WriteLine($"Subsection 4 Offset: 0x{navWorld.subsection4Offset.ToString("x")}");
                Console.WriteLine($"Subsection 3 Offset: 0x{navWorld.subsection3Offset.ToString("x")}");
                Console.WriteLine($"u1: 0x{navWorld.u1.ToString("x")}");
                Console.WriteLine($"u2: 0x{navWorld.u2.ToString("x")}");
                Console.WriteLine($"Subsection 5 Offset: 0x{navWorld.subsection5Offset.ToString("x")}");
                Console.WriteLine($"u3: 0x{navWorld.u3.ToString("x")}");
                Console.WriteLine($"Subsection 6 Offset: 0x{navWorld.subsection6Offset.ToString("x")}");
                Console.WriteLine($"Subsection 1 Entry Count: 0x{navWorld.subsection1EntryCount.ToString("x")}");
                Console.WriteLine($"Subsection 4 Entry Count: 0x{navWorld.subsection4EntryCount.ToString("x")}");
                Console.WriteLine($"Subsection 5 Entry Count: 0x{navWorld.subsection5EntryCount.ToString("x")}");

                int count = navWorld.navWorldSubsection1Entries.Length;

                for(int j = 0; j < count; j++)
                {
                    NavWorldSubsection1Entry entry = navWorld.navWorldSubsection1Entries[j];

                    Console.WriteLine($"\nSubsection 1 Entry: {j}");
                    Console.WriteLine("----------------------------------------------------------------");
                    Console.WriteLine($"u1a: 0x{entry.u1a.ToString("x")}");
                    Console.WriteLine($"u1b: 0x{entry.u1b.ToString("x")}");
                    Console.WriteLine($"u1: 0x{entry.u1.ToString("x")}");
                    Console.WriteLine($"u2a: 0x{entry.u2a.ToString("x")}");
                    Console.WriteLine($"u2b: 0x{entry.u2b.ToString("x")}\n");
                } //for

                count = navWorld.navWorldSubsection2Entries.Length;

                for (int j = 0; j < count; j++)
                {
                    NavWorldSubsection2Entry entry = navWorld.navWorldSubsection2Entries[j];

                    Console.WriteLine($"\nSubsection 2 Entry: {j}");
                    Console.WriteLine("----------------------------------------------------------------");
                    Console.WriteLine($"u1: 0x{entry.u1.ToString("x")}");
                    Console.WriteLine($"u2: 0x{entry.u2.ToString("x")}");
                    Console.WriteLine($"u3: 0x{entry.u3.ToString("x")}");
                    Console.WriteLine($"u4: 0x{entry.u4.ToString("x")}\n");
                } //for

                count = navWorld.navWorldSubsection3Entries.Length;

                Console.WriteLine($"\nSubsection 3 Entries");
                Console.WriteLine("----------------------------------------------------------------");

                for (int j = 0; j < count; j++)
                {
                    Console.Write($"0x{navWorld.navWorldSubsection3Entries[j].ToString("x")}, ");
                } //for

                Console.WriteLine($"\n");

                count = navWorld.navWorldSubsection4Entries.Length;

                for (int j = 0; j < count; j++)
                {
                    NavWorldSubsection4Entry entry = navWorld.navWorldSubsection4Entries[j];

                    Console.WriteLine($"\nSubsection 4 Entry: {j}");
                    Console.WriteLine("----------------------------------------------------------------");
                    Console.WriteLine($"u1: 0x{entry.u1.ToString("x")}");
                    Console.WriteLine($"u2: 0x{entry.u2.ToString("x")}");
                    Console.WriteLine($"u3: 0x{entry.u3.ToString("x")}");
                    Console.WriteLine($"u4: 0x{entry.u4.ToString("x")}\n");
                } //for

                count = navWorld.navWorldSubsection5Entries.Length;

                for (int j = 0; j < count; j++)
                {
                    NavWorldSubsection5Entry entry = navWorld.navWorldSubsection5Entries[j];

                    Console.WriteLine($"\nSubsection 5 Entry: {j}");
                    Console.WriteLine("----------------------------------------------------------------");
                    Console.WriteLine($"u1: 0x{entry.u1.ToString("x")}");
                    Console.WriteLine($"u2: 0x{entry.u2.ToString("x")}\n");
                } //for

                count = navWorld.navWorldSubsection6Entries.Length;

                Console.WriteLine($"\nSubsection 6 Entries");
                Console.WriteLine("----------------------------------------------------------------");

                for (int j = 0; j < count; j++)
                {
                    Console.Write($"0x{navWorld.navWorldSubsection6Entries[j].ToString("x")}, ");
                } //for

                Console.WriteLine($"\n");
            } //for
        } //DisplayNavWorldInfo
    } //class
} //namespace
