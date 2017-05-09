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
            var playlist = GetPlaylist(player, playlistName);
            playlist.clear();

            foreach (var media in files.Select(player.newMedia))
            {
                playlist.appendItem(media);
            }

            return playlist;
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

        public static void ClearPlaylist(this AxWindowsMediaPlayer player, string playlistName)
        {
            var playlist = GetPlaylist(player, playlistName);
            playlist.clear();
        }

        //TODO: PurgeLibrary() temporarily removed - might not be needed
        //private static void PurgeLibrary(this AxWindowsMediaPlayer player)
        //{
        //    // clear the media player library 
        //    var library = player.mediaCollection;
        //    var allItems = library.getAll();
        //    var countItems = allItems.count;

        //    for (var i = 0; i < countItems; i++)
        //    {
        //        var item = allItems.Item[i];
        //        library.remove(item, true);
        //    }
        //}

        private static IWMPPlaylist GetPlaylist(this AxWindowsMediaPlayer player, string playlistName)
        {
            IWMPPlaylist playlist;

            _playlistCollection = player.playlistCollection.getByName(playlistName);

            switch (_playlistCollection.count)
            {
                case 0:
                    playlist = player.playlistCollection.newPlaylist(playlistName);
                    break;
                default:
                    playlist = _playlistCollection.Item(0);
                    break;
            }

            return playlist;
        }
    }
}
