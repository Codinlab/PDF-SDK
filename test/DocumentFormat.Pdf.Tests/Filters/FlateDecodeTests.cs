﻿using DocumentFormat.Pdf.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DocumentFormat.Pdf.Tests.Filters
{
    public class FlateDecodeTests
    {
//        byte[] EncodedData = { 0x78, 0x9c, 0x63, 0x60, 0x20, 0x82, 0xff, 0xff, 0x19, 0x81, 0xa4, 0x20, 0x03, 0x03, 0x88, 0x5a, 0x06, 0xa1, 0xee, 0x81, 0x29, 0xc6, 0x37, 0x60, 0x8a, 0x59, 0x13, 0x42, 0x5d, 0x04, 0x53, 0x2c, 0x07, 0x20, 0xd4, 0x57, 0x30, 0xc5, 0xaa, 0xc5, 0xc0, 0xc0, 0x04, 0xd6, 0xce, 0x04, 0xa1, 0x98, 0x21, 0x14, 0x0b, 0x84, 0x62, 0x85, 0x50, 0x8c, 0x10, 0x0a, 0xaa, 0x92, 0x0d, 0xa8, 0x8f, 0x8d, 0x17, 0xac, 0x9d, 0x7d, 0x26, 0x98, 0xe2, 0xa8, 0x02, 0x51, 0x4c, 0xde, 0xe2, 0x60, 0x2a, 0x3c, 0x89, 0x81, 0x01, 0x20, 0xd9, 0xf6, 0x0a, 0x69 };
        byte[] EncodedData = { 0x68, 0xde, 0x62, 0x62, 0x64, 0x10, 0x60, 0x60, 0x62, 0x60, 0x8a, 0x03, 0x12, 0x0c, 0x4d, 0x40, 0x82, 0xf1, 0x1f, 0x88, 0x1b, 0x0d, 0x24, 0x98, 0xcf, 0x81, 0xb8, 0x4a, 0x40, 0xe2, 0xbb, 0x35, 0x03, 0x13, 0x23, 0xc3, 0x0a, 0x90, 0x12, 0x06, 0x46, 0xdc, 0xc4, 0x7f, 0xc6, 0x35, 0xbf, 0x20, 0x02, 0x0c, 0x20, 0x1d, 0x8f, 0x07, 0xeb };

        [Fact]
        public void DecodesData()
        {
            // Arrange
            var filter = new FlateDecode();

            // Act
            var decoded = filter.Decode(EncodedData);

            // Assert
            Assert.NotNull(decoded);
        }

        [Fact]
        public void EncodeDecode()
        {
            var data = new byte[] { 0x00, 0x01, 0x02, 0x03 };

            var filter = new FlateDecode();
            var encoded = filter.Encode(data);
            var decoded = filter.Decode(encoded);

            Assert.Equal(data, decoded);
        }
    }
}