﻿using System.Globalization;

namespace WutheringWavesTool.Common;

public static class GameIcon
{
    //public static string Icon2 =>
    //    "data:image/png;base64,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";
    //public static string Icon1 =>
    //    "data:image/png;base64,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";

    //public static string Icon3 =>
    //    "data:image/png;base64,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";

    //public static string Icon4 =>
    //    "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAFoAAABaCAYAAAA4qEECAAAACXBIWXMAACE4AAAhOAFFljFgAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAqcSURBVHgB7ZsLcFRnFcfPuXdfSSClYAUREKuMj2JpKc88oLRAeSSbhRKcIrVFqFMfreN0qjJQEeyMjqJtLbQF0WpbW4dUSBZqhlJNYXcp7UAdKRUVoSqPllKUV7J7d/fe4/kSEvZx72az3pssM99vJnP3fo97b/777fnOOd93ASQSiUQikUgkEolEIpFIJBKJRCKRSCQSicRBAvTayNn0ey8UMf5Y+FPgMAo4jN6mXePWyrfeTC0+KEIC8fBSBegr4DCOC42qawACzB6guYM1J4KlUET44+F7yYCNRPghcBhHhb5p3z43Hx4Rnwlghjpo4PaZ9OcyKAICsfB9aMATIDRAWBRIRCaDgzgq9PDR2jIezdelFE0r0S40+0+H+0Mf4o+FHuAv/jH+iJeKkJL0E3AQx4SeSTvKDDC+a1JVjf1gx/T/7LwK+oC6ttB3EHAtXBa5A4TJAW1PABzCMaF9Wr97+R8aYlrJ/1RZSenLc8+GroZexK9FVoKCP7BuYTwEDuGI0PV00INA38zdij7p9iZ7PKrr32/px6NyIhQAP9PYXPVEMLYuFrkNHMARobXY2Xo+fDRHE0KExY0l0/4JPSTeX60HFb8KBdDaGl3ChyO5W9H94ACOCI1IX+6myYZGX1UzFHRxZQm7MAtm095y6CGvDJxxziBlKX80rFvRrPnRyMfAZmwXOhBtGclqVFs2IPoAo4nlUAA1sV2j+FDFf6W+mP55KIBtJRW7+PBL6xaoJMG4E2zGdqENdN0OmTN6Coi4pvHqaWehABRSl3ReWwf6EhQIJV0r+dBq2QBxAdiM7UKz95/DRcJTmnfwRiiAetqsssRf7LoSwqR52mufgQII9pt0ioA2WdXzNzlmbjR0LdiIrULPPrO3nAOBSZYNiDY04ygNCiChDZ2JGROsQfoSKBAFlMehPWA1x4U4A2zEVqHVMl2I7LKo5pSC8iwUCJmZCqI7OVnlggJo9FUe4YvutWyAWAk2Yq/QhnGjZSXCW0FfxT8yi2vaQlX+aPguyEHN+RZO+mBt9jVxSLnmnpOrr5icA9HwQxbPtMWqH1Fun7un2GujVbDM6yJhS1Yhj3FVwfVsb39RF91taQZUj2sRH0xz2hx9Wvarje76OKGnxUBYXdO6O0s4VFwtVn3ZTI1aRWSbPvYKTTjcsg5hf2ZRXSx8Kx+uB/EVIW7yx0OmngQb0hweBs2dR5EPZ5aKyUxBlYWkkSiytYryjcw2x9yuA3xIWFzY81ZbaDDYhK1C88gcaFWn63TYpEeKL4wKGvjzumh4WWqL+fHdY3nUjgFr3LqW7vcGYpFP8GQmRuvlwAMhMPtw+krPfhwnRH7H6sIJl2L5//QUW4Vmu+a2qmMTcTqrEHE6ZD0P3ZNaoJNSD93AvnlGGxJ2e0RGs3L3sLLsHAnBGbDAnTRsW4Kz13QgtllVxZPxtADBT3sHi591RjMDVUjPY3g8T3Cy5zRYY7Cp/35qgdt7UiT0D0DW46kTsnqjdeCSVBXroKaH2Gyj4ZxVlUf1pa8ZxpKfzWqEGGz0VKfZ8iaccIxzJ0vB8pbws6Cv8qXUsgZcqBsGrjFpfJ3JFTxgRdJVUARrht1CH7WsomTaxMIpy+Em/Z8y69vkq97GHZ43qXrH8PpWmPXxlVQ08k3fhfSbDstuaZkzvxgsm/g+2ITdIfh+yxrMcP0QB2U0OHfOG/+DVfe4nvg2H9JMk4H44HYcZ2quGhB18QvJdU+RN+fDSLP+7EG9KdKQYBO2Cp0E+qNlJWJF2rlBaRMNR35vvIrTklbdm0unHec261I67N/mrfwd5IAnyUhaAVFJ6mkselb41uamA8nySy8EW4V+qaT6qBDAonp2+p0xTVQ2JYehGxIJEmt90Y7+xg+7a88h/98zivTUE1WlWda9jRfBRmzP3hHgBouqEYG2y0v6HCmeT++I3U48zf2nnObf8vP8d/QGT/WWbp/F0M+mPxv89/IJoUGwyLwj7WnyTvkL2IjtQns/wOf4cMKsjkPh+7pOkI6l1hFCXptrOCjexDbhmdWIRndtMamnmwXErnsGtMgsDoRGmfZDNccCbmHYLnTD8IooBy5m2wzE3LJw7oXI6PYTShxKqyMaAnmwzV31uhqHJ/Npy4F3umdjUMc9O0bzaoterzb6KraDzTiyZhj0VT3NQUbYpEpVXcY68Y+2L8wSdLlPvFo7Ia+LszHf2r8yL7cLFUq7poL4ujgGYuF7OF0w3qSLxvmRghZ+u8OZfR08dHmpSaQ+z2dX4VT+2XYkeBTc0VUOcG1ta2Q82Aibo/ldn4FaB3jju8W6I3+pVruSlm/1Tj4EDuDYBpp2DwRRRHRZvigX/CgQDd3MsfMLqeWKQl8Hm6hNRG7hr+9zneeIypaTMFBVQG3g037ZPaixyVv5KDiEo3vv+MFf5FH1sEmVmxTcgoryHg/ly7M70mI7NhuKLJ2iG6micWZZ31CiXdgi1gOze9CBeOuFu+wMUDJxfNtu0FO1isX+dVYFwdU8I+0kA1KCDraqOr0wv3X3R6BQ2L/0DC9flzqa+XMz6oqYoGea9Pi3bkBt86A558FBEHoBsa53lebin6zpCnkrj7coj6bUPcp/Y9drbvu6Xg+4ifa5h2nR9Xyf1FSrGKUnwWznFE/GuqJM3e6t+Cs4TK8ILRB5hbh2VkRbtfm0Z3XOs4Ox/AZv1VP5+Mw1F0NjVBU2Qp7ei0i9co78lq3eyoPQC/Sa0IJ62sxiD/0tf5yXdyeEt9lTWdvoqfyVWbVf23s9GMmV2OFhqJAf7xkJmL6tf9Xb0Es4bqNT4Txx/Jw3sZCH62/y7kQwiO225ZYChY0viyzMTr4i/0sH15TeFFnQqyO6C1qlBLQZj7F56M6dO6V6XeO24KTj3V3PH5/5HEeXd+RsB3iIR/9tTaVTj0Ev0zdCX8IfDa1is/C9HE0ebPJVrYV8rtUaHsrLYEJA818p0Rvo88xpxIlnoA/oVdORSbCkmvMNIkgh08mOo7m8J6pgWdVJzrGYisijaafHl7y1r0QW9KnQAl6mWk8GfYE/xjPr2MXLe5+yeBOAfx1Zr2qwd7HZ7R1Q04DTLkIf0udCC4KlU9gTMfyQuZUW4W7Ik3i5S+RW0iZNjl2evNFXeUcDjo5DH9OnNjqTeW2hKkNVtvMw7Hq3hR/w/kZf1eO5+onXoCmuvymizZTiH7N9/xYUCUUxojvZWlod5sWBGSxY10oI2+lH/NHIYqs+NRQaQZq+I01kgjXFJLKgqEZ0JyJdqqjGTn68zpEtwuhHPV5lRQNWRDvb+WORuTz6n2Y/+prOMp4QH+ZJ1rHX2AqlKIUW1MbDkzjp9ApPiKmvNB/hJ15hxOGg4gaxOyktwuTR/9Ogr/oBKEKKVmhBIBaZyeJtA6stAakgPNPkqbzbyVTn/0NR2ehMOHv3MhmG8CZyJpVY2Z3HPb5lxSqyoKiFFrS7fhaLvQL2k/eBF26/tAW3aClq05GKXws/y+M10/t4VzeU8dtLK05AkVP0I7oTr0cRb+P+KaUoAYa+4EoQWXDFCC3cOh3a35a9IM7ZGK/gLNwekDhDXSz0tTotErHzRR6JCfVEao3W8mmQSCQSiUQikUgkEolEIpFIJBLJlcT/AGonfw6+w6bZAAAAAElFTkSuQmCC";

    //public static string Icon5 =>
    //    "data:image/png;base64,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";

    //public static string Icon6 =>
    //    "data:image/png;base64,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";


    public static string Icon1 => "ms-appx:///Assets/Icons/Icon1.png";
    public static string Icon2 => "ms-appx:///Assets/Icons/Icon2.png";
    public static string Icon3 => "ms-appx:///Assets/Icons/Icon3.png";
    public static string Icon4 => "ms-appx:///Assets/Icons/Icon4.png";
    public static string Icon5 => "ms-appx:///Assets/Icons/Icon5.png";
    public static string Icon6 => "ms-appx:///Assets/Icons/Icon6.png";
}