using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class BinaryReaderMac : BinaryReader {
	public BinaryReaderMac(Stream stream) : base(stream) { }

	public override ushort ReadUInt16() {
		var data = base.ReadBytes(2);
		Array.Reverse(data);
		return BitConverter.ToUInt16(data, 0);
	}
}
