namespace Fallout.Fusion.Test;

public class PffUtilitiesTests {
    [Fact]
    public void GetPffString_ShouldReturnVersionedMarker() {
        string result = PffUtilities.GetPffString();

        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldStartWith("[PFF] (");
        result.ShouldEndWith(")");
    }

    [Fact]
    public void GetPffString_ShouldReturnCachedValue() {
        string first = PffUtilities.GetPffString();
        string second = PffUtilities.GetPffString();

        second.ShouldBeSameAs(first);
    }
}
