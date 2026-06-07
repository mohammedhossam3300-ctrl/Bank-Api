namespace Bank.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1_ShouldPass()
    {
        // Arrange
        int expected = 1;
        
        // Act
        int actual = 1;
        
        // Assert
        Assert.Equal(expected, actual);
    }
}
