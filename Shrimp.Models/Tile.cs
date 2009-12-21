using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shrimp.Models
{
    public struct Tile
    {
        private byte TileSetIdLower;
        private byte TileSetIdUpper;
        private byte TileIdLower;
        private byte TileIdUpper;

        public short TileSetId
        {
            get
            {
                return (short)((this.TileSetIdUpper << 8) | this.TileSetIdLower);
            }
            set
            {
                this.TileSetIdLower = (byte)value;
                this.TileSetIdUpper = (byte)(value >> 8);
            }
        }
        public short TileId
        {
            get
            {
                return (short)((this.TileIdUpper << 8) | this.TileIdLower);
            }
            set
            {
                this.TileIdLower = (byte)value;
                this.TileIdUpper = (byte)(value >> 8);
            }
        }

        public byte[] ToBytes()
        {
            return new[]
            {
                this.TileSetIdLower,
                this.TileSetIdUpper,
                this.TileIdLower,
                this.TileIdUpper,    
            };
        }

        public void FromBytes(byte[] bytes, int index)
        {
            this.TileSetIdLower = bytes[index];
            this.TileSetIdUpper = bytes[index + 1];
            this.TileIdLower = bytes[index + 2];
            this.TileIdUpper = bytes[index + 3];
        }

        public override int GetHashCode()
        {
            return (this.TileSetIdLower << 24) | (this.TileSetIdUpper << 16) |
                (this.TileIdLower << 8) | this.TileIdUpper;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (!(obj is Tile))
            {
                return false;
            }
            else
            {
                return this.Equals((Tile)obj);
            }
        }

        public bool Equals(Tile tile)
        {
            return this.TileSetId == tile.TileSetId && this.TileId == tile.TileId;
        }

        public static bool operator ==(Tile tile1, Tile tile2)
        {
            return tile1.Equals(tile2);
        }

        public static bool operator !=(Tile tile1, Tile tile2)
        {
            return !(tile1.Equals(tile2));
        }

        public override string ToString()
        {
            return string.Format("{{TileSetId:{0},TileId:{1}}}", this.TileSetId, this.TileId);
        }
    }
}
