namespace Quantum.QuantumPerceptron
{
    open Microsoft.Quantum.Intrinsic;
    open Microsoft.Quantum.Canon;
	open Microsoft.Quantum.Convert;
	open Microsoft.Quantum.Arrays;

	/// <summary>
    /// Quantum Operation Block for Initiating Perceptron
	/// implementation using inputvector and weightvector
    /// </summary>
    /// <param name="inputVector">Input Vector</param>
    /// <param name="weightVector">Weight Vector</param>
    operation Perceptron(inputVector : Bool [], weightVector: Bool [], qubitCount: Int, iterations: Int) : Int {
		
		// inpVec and wtVec will be of size 15.
		using(qbits = Qubit[qubitCount+1])
		{
			mutable countOne = 0;

			for(i in 1..iterations)
			{
				ApplyToEach(H, qbits[0..qubitCount-1]);
				ApplyVector(qbits[0..qubitCount-1], inputVector);
				ApplyVector(qbits[0..qubitCount -1], weightVector);
				ApplyToEach(H, qbits[0..qubitCount -1]);
				ApplyToEach(X, qbits[0..qubitCount -1]);
				Controlled X(qbits[0..qubitCount - 1], qbits[qubitCount]);

				let r = M(qbits[qubitCount]);

				if(r == One)
				{
					set countOne = countOne + 1;
				}

				ResetAll(qbits);
			}

			return countOne;
		}
    }

	// Quantum Operation to apply Controlled Z or Z Gate on Qubits base don Input vector
	operation ApplyVector(qbits: Qubit[], vector: Bool []) : Unit {
		let vectorSize = Length(vector);
		// Generalized method to derive Z Gate
		// position using exponent calculation

		mutable i = 0;
		repeat {
			// Check if current vector index is true
			// and valid for processing
			if (vector[i]) {
				ProcessIndexToApplyZ(i+1, qbits);
			}

			set i += 1;
		}
		until (i > vectorSize-1);
	}

	// Operation to apply Z on provided Vector Index value
	operation ProcessIndexToApplyZ(index: Int, qubits: Qubit[]): Unit {
		// Take an empty Qubit Array to store the sub array of qubit pointers
		mutable qubitIndex = new Int[0];
		mutable iter = index;
		mutable maxExponent = -1;

		repeat {
			set maxExponent = GetMaximumExponent(iter);
			set qubitIndex += [maxExponent];
			set iter -= 2^maxExponent;
		}
		until (iter < 1);

		let qubitIndexLength = Length(qubitIndex);

		// Apply Z Gate if the size is equal to 1
		if (qubitIndexLength == 1) {
			Z(qubits[qubitIndex[0]]);
		}

		// Apply Controlled Z is More than 2 indexes are found in QubitIndex array
		if (qubitIndexLength > 1) {
			ControlledZ(Subarray(qubitIndex[1..qubitIndexLength-1], qubits), qubits[qubitIndex[0]]);
		}
	}

	// Method to get maximum exponent from the provided index
	// which is just smaller or equal to the value of index itself
	operation GetMaximumExponent(index: Int): Int {
		mutable result = 0;
		
		for (i in 0..index) {
			
			// Calculating exponent to the base of 2
			let exponent = 2^i;
			
			// Return exponent if the index is exponent
			// if equal to the index
			if (exponent == index) {
				return i;
			}
			
			// If the exponent if greater than the index then
			// return the previous exponent which was less than the index
			// from the value of result multable element
			if(exponent > index) {
				return result;
			}

			// Setting exponent to the value of result
			// if the exponent is less than index value
			set result = i;
		}

		return 0;
	}

	/// Summary
	/// Method to implement Controlled Z
	/// Summary
	operation ControlledZ(qubits : Qubit [], controlledBit: Qubit): Unit {
		Controlled Z(qubits, controlledBit);
	}
}