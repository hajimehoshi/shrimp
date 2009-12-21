using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Shrimp;
using Shrimp.Models;

namespace Shrimp.Tests
{
    [TestFixture]
    public class TileTest
    {
        [Test]
        public void Test()
        {
            Tile tile1 = new Tile
            {
                TileSetId = 1,
                TileId = 2,
            };
            Tile tile2 = new Tile
            {
                TileSetId = 1,
                TileId = 2,
            };
            Tile tile3 = new Tile
            {
                TileSetId = 1,
                TileId = 3,
            };
            Assert.IsTrue(Tile.Equals(tile1, tile2));
            Assert.IsFalse(Tile.Equals(tile1, tile3));
            Assert.IsFalse(Tile.Equals(tile1, null));
            Assert.IsTrue(tile1.Equals(tile2));
            Assert.IsFalse(tile1.Equals(tile3));
            Assert.IsFalse(tile1.Equals(null));
            Assert.IsTrue(tile1 == tile2);
            Assert.IsFalse(tile1 != tile2);
            Assert.IsFalse(tile1 == tile3);
            Assert.IsTrue(tile1 != tile3);
            Assert.IsFalse(tile1 == null);
            Assert.IsTrue(tile1 != null);
        }

        [Test]
        public void TestAssign()
        {
            Tile tile1 = new Tile
            {
                TileSetId = 1,
                TileId = 2,
            };
            Tile tile2 = tile1;
            Assert.AreEqual(1, tile2.TileSetId);
            Assert.AreEqual(2, tile2.TileId);
        }

        [Test]
        public void TestFromBytes()
        {
            Tile tile = new Tile();
            tile.FromBytes(new byte[] { 1, 2, 3, 4 }, 0);
            Assert.AreEqual((2 << 8) | 1, tile.TileSetId);
            Assert.AreEqual((4 << 8) | 3, tile.TileId);
            tile.FromBytes(new byte[] { 1, 2, 3, 4, 5 }, 1);
            Assert.AreEqual((3 << 8) | 2, tile.TileSetId);
            Assert.AreEqual((5 << 8) | 4, tile.TileId);
        }
    }
}
