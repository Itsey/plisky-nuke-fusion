namespace Nuke.Fusion.Test;

public class PnfUtilitiesTests {
    [Fact]
    public void GetPnfString_ShouldReturnVersionedMarker() {
        string result = PnfUtilities.GetPnfString();

        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldStartWith("[PNF] (");
        result.ShouldEndWith(")");
    }

    [Fact]
    public void GetPnfString_ShouldReturnCachedValue() {
        string first = PnfUtilities.GetPnfString();
        string second = PnfUtilities.GetPnfString();

        second.ShouldBeSameAs(first);
    }
}
