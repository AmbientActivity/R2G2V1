using System.Collections.Generic;
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
            // load the new playlist
            var playlist = GetPlaylist(player, playlistName);
            foreach (var media in files.Select(player.newMedia))
            {
                playlist.appendItem(media);
            }

            return playlist;
        }

        public static void PurgeLibrary(this AxWindowsMediaPlayer player)
        {
            // clear the media player library 
            var library = player.mediaCollection;
            var allItems = library.getAll();
            var countItems = allItems.count;

            for (var i = 0; i < countItems; i++)
            {
                var item = allItems.Item[i];
                library.remove(item, true);
            }
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

        public static IWMPPlaylist GetPlaylist(this AxWindowsMediaPlayer player, string playlistName)
        {
            IWMPPlaylist playlist = null;

            _playlistCollection = player.playlistCollection.getByName(playlistName);

            var count = _playlistCollection.count;
            if (count == 0) return player.playlistCollection.newPlaylist(playlistName);
            
            // remove all but last one
            if (count >= 1)
            {
                playlist = _playlistCollection.Item(0);
                playlist.clear();
            }
      
            return playlist;
        }
    }
}
