using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shrimp;
using Shrimp.Models;

namespace Shrimp.Tests
{
    [TestFixture]
    public class MapTest
    {
        [Test]
        public void TestJson()
        {
            ViewModel viewModel = new ViewModel();
            MapCollection mapCollection = new MapCollection(viewModel);
            Map map1 = new Map(mapCollection, 1);
            map1.Width = 100;
            map1.Height = 200;
            map1.CreateSettingTilesCommand(0, 1, 2,
                SelectedTiles.Single(new Tile { TileSetId = 3, TileId = 4 }), 0, 0).Do();
            map1.CreateSettingTilesCommand(1, 5, 6,
                SelectedTiles.Single(new Tile { TileSetId = 7, TileId = 8 }), 0, 0).Do();
            JToken token = map1.ToJson();
            Assert.AreEqual(map1.Width, token["Width"].Value<int>());
            Assert.AreEqual(map1.Height, token["Height"].Value<int>());
            Assert.AreEqual(2, token["Tiles"].Count());
            byte[] bytes1 = Convert.FromBase64String(token["Tiles"][0].Value<string>());
            Assert.AreEqual(map1.Width * map1.Height * 4, bytes1.Length);
            Assert.AreEqual(3, bytes1[(1 + 2 * map1.Width) * 4]);
            Assert.AreEqual(0, bytes1[(1 + 2 * map1.Width) * 4 + 1]);
            Assert.AreEqual(4, bytes1[(1 + 2 * map1.Width) * 4 + 2]);
            Assert.AreEqual(0, bytes1[(1 + 2 * map1.Width) * 4 + 3]);
            byte[] bytes2 = Convert.FromBase64String(token["Tiles"][1].Value<string>());
            Assert.AreEqual(map1.Width * map1.Height * 4, bytes2.Length);
            Assert.AreEqual(7, bytes2[(5 + 6 * map1.Width) * 4]);
            Assert.AreEqual(0, bytes2[(5 + 6 * map1.Width) * 4 + 1]);
            Assert.AreEqual(8, bytes2[(5 + 6 * map1.Width) * 4 + 2]);
            Assert.AreEqual(0, bytes2[(5 + 6 * map1.Width) * 4 + 3]);

            Map map2 = new Map(mapCollection, 2);
            Assert.AreEqual(Map.MinWidth, map2.Width);
            Assert.AreEqual(Map.MinHeight, map2.Height);
            map2.LoadJson(token);
            Assert.AreEqual(map1.Width, map2.Width);
            Assert.AreEqual(map1.Height, map2.Height);
            Assert.AreEqual(map1.GetTile(0, 0, 0), map2.GetTile(0, 0, 0));
            Assert.AreEqual(map1.GetTile(0, 1, 2), map2.GetTile(0, 1, 2));
            Assert.AreEqual(map1.GetTile(1, 5, 6), map2.GetTile(1, 5, 6));
        }

        [Test]
        public void TestUndo()
        {
            ViewModel viewModel = new ViewModel();
            MapCollection mapCollection = new MapCollection(viewModel);
            Map map1 = new Map(mapCollection, 1);
            map1.Width = 100;
            map1.Height = 100;
            Tile[] tiles1 = new[]
            {
                new Tile { TileSetId = 1, TileId = 2 },
                new Tile { TileSetId = 3, TileId = 4 },
                new Tile { TileSetId = 5, TileId = 6 },
                new Tile { TileSetId = 7, TileId = 8 },
            };
            Tile[] tiles2 = new[]
            {
                new Tile { TileSetId = 101, TileId = 102 },
                new Tile { TileSetId = 103, TileId = 104 },
                new Tile { TileSetId = 105, TileId = 106 },
                new Tile { TileSetId = 107, TileId = 108 },
            };
            Command command1 = map1.CreateSettingTilesCommand(1, 2, 3, SelectedTiles.Picker(tiles1, 2, 2), 0, 1);
            Assert.AreEqual(new Tile { TileSetId = 0, TileId = 0 }, map1.GetTile(0, 0, 0));
            Assert.AreEqual(new Tile { TileSetId = 0, TileId = 0 }, map1.GetTile(0, 1, 0));
            Assert.AreEqual(new Tile { TileSetId = 0, TileId = 0 }, map1.GetTile(0, 0, 1));
            Assert.AreEqual(new Tile { TileSetId = 0, TileId = 0 }, map1.GetTile(0, 1, 1));
            command1.Do();
            Assert.AreEqual(tiles1[2], map1.GetTile(1, 2, 3));
            Assert.AreEqual(tiles1[3], map1.GetTile(1, 3, 3));
            Assert.AreEqual(tiles1[0], map1.GetTile(1, 2, 4));
            Assert.AreEqual(tiles1[1], map1.GetTile(1, 3, 4));
            Command command2 = map1.CreateSettingTilesCommand(1, 2, 3, SelectedTiles.Picker(tiles2, 2, 2), 1, 0);
            command2.Do();
            Assert.AreEqual(tiles2[1], map1.GetTile(1, 2, 3));
            Assert.AreEqual(tiles2[0], map1.GetTile(1, 3, 3));
            Assert.AreEqual(tiles2[3], map1.GetTile(1, 2, 4));
            Assert.AreEqual(tiles2[2], map1.GetTile(1, 3, 4));
            command2.Undo();
            Assert.AreEqual(tiles1[2], map1.GetTile(1, 2, 3));
            Assert.AreEqual(tiles1[3], map1.GetTile(1, 3, 3));
            Assert.AreEqual(tiles1[0], map1.GetTile(1, 2, 4));
            Assert.AreEqual(tiles1[1], map1.GetTile(1, 3, 4));
            command1.Undo();
            Assert.AreEqual(new Tile { TileSetId = 0, TileId = 0 }, map1.GetTile(0, 0, 0));
            Assert.AreEqual(new Tile { TileSetId = 0, TileId = 0 }, map1.GetTile(0, 1, 0));
            Assert.AreEqual(new Tile { TileSetId = 0, TileId = 0 }, map1.GetTile(0, 0, 1));
            Assert.AreEqual(new Tile { TileSetId = 0, TileId = 0 }, map1.GetTile(0, 1, 1));
        }
    }
}
