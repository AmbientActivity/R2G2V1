﻿using System.Collections.Generic;
using System.Linq;
using AxWMPLib;
using WMPLib;

namespace Keebee.AAT.Display.Extensions
{
    public static class MediaPlayerExtensions
    {
        private static IWMPPlaylistArray _playlistCollection;

        public static IWMPPlaylist LoadPlaylist(this AxWindowsMediaPlayer player, string playlistName, IEnumerable<string> files)
        {
            // clear the existing playlist
            var playlist = InitializePlaylist(player, playlistName);
            playlist.clear();

            // clear the media player library 
            var library = player.mediaCollection;
            var allItems = library.getAll();
            var countItems = allItems.count;

            for (var i = 0; i < countItems; i++)
            {
                var item = allItems.Item[i];
                library.remove(item, true);
            }

            // load the new playlist
            foreach (var media in files.Select(player.newMedia))
            {
                playlist.appendItem(media);
            }

            return playlist;
        }

        public static void ClearPlaylist(this AxWindowsMediaPlayer player, string playlistName)
        {
            var playlist = InitializePlaylist(player, playlistName);

            playlist.clear();
        }

        public static int CurrentIndex(this AxWindowsMediaPlayer player, IWMPPlaylist playlist)
        {
            var index = 0;
            if (player.currentMedia == null) return 0;

            for (var i = 0; i < playlist.count; i++)
            {
                if (!player.currentMedia.isIdentical[playlist.Item[i]]) continue;

                index = i;
                break;
            }
            return index;
        }

        private static IWMPPlaylist InitializePlaylist(AxWindowsMediaPlayer player, string playlistName)
        {
            _playlistCollection = player.playlistCollection.getByName(playlistName);

            return _playlistCollection.count == 0
                    ? player.playlistCollection.newPlaylist(playlistName)
                    : _playlistCollection.Item(0);
        }
    }
}
