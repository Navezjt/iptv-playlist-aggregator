using NUnit.Framework;

using Moq;

using IptvPlaylistAggregator.Service;
using IptvPlaylistAggregator.Service.Models;

namespace IptvPlaylistAggregator.UnitTests.Service.Models
{
    public sealed class ChannelMatcherTests
    {
        Mock<ICacheManager> cacheMock;

        IChannelMatcher channelMatcher;

        [SetUp]
        public void SetUp()
        {
            cacheMock = new Mock<ICacheManager>();

            channelMatcher = new ChannelMatcher(cacheMock.Object);
        }

        [TestCase("Agro TV", "RO: Agro", "Agro RO")]
        [TestCase("Antena 1", "RO: Antenna", "RO: Antenna HD")]
        [TestCase("Ardeal TV", "RO: Ardeal TV", "|RO| Ardeal TV")]
        [TestCase("Bollywood TV", "RO: BO TV", "BO TV")]
        [TestCase("Cartoon Network", "RO: Cartoon Network", "VIP|RO|: Cartoon Network")]
        [TestCase("CineMAX 1", "RO: CineMAX", "CineMAX RO")]
        [TestCase("Digi Sport 2", "RO: Digi Sport 2", "RO: DIGI Sport 2")]
        [TestCase("Digi Sport 2", "RO: Digi Sport 2", "Romanian:DIGI Sport 2")]
        [TestCase("Digi World", "RO: Digi World FHD", "RUMANIA: DigiWorld FHD (Opt-1)")]
        [TestCase("Duna", "RO: Duna TV", "RO | Duna Tv")]
        [TestCase("Golf Channel", "FR: Golf Channel", "|FR| GOLF CHANNEL FHD")]
        [TestCase("H!T Music Channel", "RO: Hit", "RO | HIT")]
        [TestCase("HD Net Van Damme", "HD NET Jean Claude Van Damme", "HD NET Jean Claude van Damme")]
        [TestCase("Jurnal TV", "MD: Jurnal TV", "Jurnal TV Moldavia")]
        [TestCase("MegaMax", "RO: MegaMax", "RO: MegaMax-HD")]
        [TestCase("NCN TV", "RO: NCN", "RO: NCN HD")]
        [TestCase("Pro TV News", "RO: Pro News", "Pro News")]
        [TestCase("Publika TV", "MD: Publika", "PUBLIKA_TV_HD")]
        [TestCase("România TV", "România TV", "RO\" Romania TV")]
        [TestCase("Somax", "RO: Somax TV", "Somax TV")]
        [TestCase("Sundance", "RO: Sundance TV", "RO: Sundance TV FHD (MultiSub)")]
        [TestCase("Sundance", "RO: Sundance TV", "RO: Sundance TV FHD [Multi-Sub]")]
        [TestCase("Travel Channel", "RO: Travel", "RO | Travel")]
        [TestCase("Travel Mix", "RO: Travel Mix TV", "Travel Mix TV RO")]
        [TestCase("TV Paprika", "RO: Paprika TV", "RO TV Paprika")]
        [TestCase("TV8", "MD: TV8", "TV 8 Moldova HD")]
        [TestCase("TVC21", "MD: TVC21", "TVC 21 Moldova")]
        [TestCase("TVR Moldova", "RO: TVR Moldova", "RO: TVR Moldova")]
        [TestCase("TVR Târgu Mureș", "RO: TVR T?rgu-Mure?", "TVR: Targu Mureș")]
        [TestCase("VSV De Niro", "VSV Robert de Niro", "VSV Robert de Niro HD")]
        [Test]
        public void ChannelNamesDoMatch_WithAliasWithoutCountry(
            string definedName,
            string alias,
            string providerName)
        {
            ChannelName channelName = GetChannelName(definedName, alias);

            Assert.IsTrue(channelMatcher.DoesMatch(channelName, providerName, country2: null));
        }

        [TestCase("AMC", "RO: AMC Romania")]
        [TestCase("Antena 3", "Antena 3 Ultra_HD")]
        [TestCase("HBO 3", "HBO 3 F_HD")]
        [TestCase("MTV Europe", "RO: MTV Europe")]
        [TestCase("Pro TV", "PRO TV ULTRA_HD")]
        [TestCase("Realitatea Plus", "Realitatea Plus")]
        [TestCase("TVR", "RO: TVR HD (1080P)")]
        [TestCase("U TV", "UTV")]
        [TestCase("Vivid TV", "Vivid TV HD(18+)")]
        [Test]
        public void ChannelNamesDoMatch_WithoutAliasWithoutCountry(
            string definedName,
            string providerName)
        {
            ChannelName channelName = GetChannelName(definedName, alias: null);

            Assert.IsTrue(channelMatcher.DoesMatch(channelName, providerName, country2: null));
        }

        [TestCase("Cromtel", "Cmrotel", "Cmtel")]
        [TestCase("Telekom Sport 2", "RO: Telekom Sport 2", "RO: Digi Sport 2")]
        [Test]
        public void ChannelNamesDoNotMatch_WithAliasWithoutCountry(
            string definedName,
            string alias,
            string providerName)
        {
            ChannelName channelName = GetChannelName(definedName, alias);

            Assert.IsFalse(channelMatcher.DoesMatch(channelName, providerName, country2: null));
        }

        [TestCase("Pro TV", "MD: ProTV Chisinau")]
        [Test]
        public void ChannelNamesDoNotMatch_WithoutAliasWithoutCountry(
            string definedName,
            string providerName)
        {
            ChannelName channelName = GetChannelName(definedName, alias: null);

            Assert.IsFalse(channelMatcher.DoesMatch(channelName, providerName, country2: null));
        }

        [TestCase(" MD| Publika", "MD", "MDPUBLIKA")]
        [TestCase("|AR| AD SPORT 4 HEVC", "AR", "ARADSPORT4")]
        [TestCase("|FR| GOLF CHANNELS HD", "FR", "FRGOLFCHANNELS")]
        [TestCase("|RO| Ardeal TV", "RO", "ARDEALTV")]
        [TestCase("|ROM|: Cromtel", "RO", "CROMTEL")]
        [TestCase("|UK| CHELSEA TV (Live On Matches) HD", "UK", "UKCHELSEATV")]
        [TestCase("Canal Regional (Moldova)", "MD", "MDCANALREGIONAL")]
        [TestCase("RO | Travel", "RO", "TRAVEL")]
        [TestCase("RO: Travel", "RO", "TRAVEL")]
        [TestCase("Travel Mix", "RO", "TRAVELMIX")]
        [TestCase("TV Paprika", "RO", "TVPAPRIKA")]
        [TestCase("TV8", "MD", "MDTV8")]
        [TestCase("TVC21", "MD", "MDTVC21")]
        [TestCase("TVR Moldova", "MD", "MDTVR")]
        [TestCase("TVR Târgu Mureș", "RO", "TVRTARGUMURES")]
        [TestCase("VSV De Niro", "RO", "VSVDENIRO")]
        [Test]
        public void NormaliseName_WithCountry_ReturnsExpectedValue(string name, string country, string expectedNormalisedName)
        {
            string actualNormalisedName = channelMatcher.NormaliseName(name, country);
            
            Assert.That(actualNormalisedName, Is.EqualTo(expectedNormalisedName));
        }

        [TestCase(" MD| Publika", "MDPUBLIKA")]
        [TestCase("|AR| AD SPORT 4 HEVC", "ARADSPORT4")]
        [TestCase("|FR| GOLF CHANNELS HD", "FRGOLFCHANNELS")]
        [TestCase("|RO| Ardeal TV", "ARDEALTV")]
        [TestCase("|ROM|: Cromtel", "CROMTEL")]
        [TestCase("|UK| CHELSEA TV (Live On Matches) HD", "UKCHELSEATV")]
        [TestCase("Canal Regional (Moldova)", "MDCANALREGIONAL")]
        [TestCase("DIGI SPORT 4 (RO)", "DIGISPORT4")]
        [TestCase("Jurnal TV Moldova", "MDJURNALTV")]
        [TestCase("MD: Canal Regional (Moldova)", "MDCANALREGIONAL")]
        [TestCase("MD: MD: [MD] Publika", "MDPUBLIKA")]
        [TestCase("MD: MD: Moldova 1", "MDMOLDOVA1")]
        [TestCase("MD: MD| Pro TV Chișinău.", "MDPROTVCHISINAU")]
        [TestCase("MD: ProTV Chisinau", "MDPROTVCHISINAU")]
        [TestCase("MINIMAX ROMANIA HD", "MINIMAXROMANIA")]
        [TestCase("Pro TV [B] RO", "PROTV")]
        [TestCase("PUBLIKA_TV_HD", "PUBLIKATV")]
        [TestCase("RO    \" DIGI SPORT 1 HD RO", "DIGISPORT1")]
        [TestCase("RO | Travel", "TRAVEL")]
        [TestCase("RO: Travel", "TRAVEL")]
        [TestCase("RO-Animal Planet HD", "ANIMALPLANET")]
        [TestCase("Ro: 1 HD", "1HD")]
        [TestCase("RO: Animal World [768p]", "ANIMALWORLD")]
        [TestCase("RO: Bit TV (ROM)", "BITTV")]
        [TestCase("RO: Digi24 (România)", "DIGI24")]
        [TestCase("RO: HBO 3 RO", "HBO3")]
        [TestCase("RO: HBO HD RO", "HBO")]
        [TestCase("RO: Nașul TV (New!)", "NASULTV")]
        [TestCase("RO: Nickelodeon (RO)", "NICKELODEON")]
        [TestCase("Ro: Pro TV backup", "PROTV")]
        [TestCase("Ro: Romania TV backup", "ROMANIATV")]
        [TestCase("RO: Tele Moldova", "TELEMOLDOVA")]
        [TestCase("RO: TVR Moldova", "TVRMOLDOVA")]
        [TestCase("RO: U TV S1-1", "UTV")]
        [TestCase("RO.| DIGI 24", "DIGI24")]
        [TestCase("RO\" Romania TV", "ROMANIATV")]
        [TestCase("RO| Antena 3 4K+", "ANTENA3")]
        [TestCase("RO| CINEMA RO.", "CINEMARO")]
        [TestCase("RO| Digi Life 4K+", "DIGILIFE")]
        [TestCase("RO| TARAF:HD", "TARAF")]
        [TestCase("RO|DISOVERY_SCIENCE_HD", "DISOVERYSCIENCE")]
        [TestCase("RTR Moldova HD", "MDRTR")]
        [TestCase("RUMANIA: DigiWorld FHD (Opt-1)", "DIGIWORLD")]
        [TestCase("TV 8 HD (Auto)", "TV8")]
        [TestCase("TV 8 Moldova HD", "MDTV8")]
        [TestCase("TV Centrală Moldova", "MDTVCENTRALA")]
        [TestCase("TVR 1 (Backup) RO", "TVR1")]
        [TestCase("TVR2 [B] RO", "TVR2")]
        [TestCase("U TV", "UTV")]
        [TestCase("US: NASA TV US", "USNASATV")]
        [TestCase("VIP|RO|: Discovery Channel FHD", "DISCOVERYCHANNEL")]
        [TestCase("VSV Robert de Niro HD", "VSVROBERTDENIRO")]
        [TestCase("VSV Robert de Niro", "VSVROBERTDENIRO")]
        [TestCase("ZonaM Moldova", "MDZONAM")]
        [Test]
        public void NormaliseName_WithoutCountry_ReturnsExpectedValue(string inputValue, string expectedValue)
        {
            string actualValue = channelMatcher.NormaliseName(inputValue, country: null);
            
            Assert.AreEqual(expectedValue, actualValue);
        }

        private ChannelName GetChannelName(string definedName, string alias)
            => GetChannelName(definedName, country: null, alias);

        private ChannelName GetChannelName(string definedName, string country, string alias)
        {
            if (alias is null)
            {
                return new ChannelName(definedName);
            }
            
            return new ChannelName(definedName, country, alias);
        }
    }
}
