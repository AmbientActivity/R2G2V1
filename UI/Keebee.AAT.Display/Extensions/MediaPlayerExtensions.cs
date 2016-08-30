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
            var playlist = InitializePlaylist(player, playlistName);

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
