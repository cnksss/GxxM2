unit LbConst;
  {-miscellaneous constants}

interface

const
  { various byte count constants }
  cBytes128 = 16;
  cBytes160 = 20;
  cBytes192 = 24;
  cBytes256 = 32;
  cBytes512 = 64;
  cBytes768 = 96;
  cBytes1024 = 128;
  cBytes2048 = 256;

  { defaults }
  cDefIterations  = 20;

{!!.06}
const
  { ASN.1 constants }
  ASN1_TYPE_SEQUENCE        = $10;
  ASN1_TYPE_Integer         = $02;
  ASN1_TAG_NUM_MASK         = $1f;
  ASN1_TYPE_HIGH_TAG_NUMBER = $1f;
  HIGH_BIT_MASK             = $80;
  BIT_MASK_7F               = $7F;



resourcestring

  { version number }
  sLbVersion = '2.07';

  { big integer errors }
  sBIBufferUnderflow   = 'Buffer UnderFlow';
  sBIBufferNotAssigned = 'Buffer not assigned';
  sBINoNumber          = 'No Number';
  sBISubtractErr       = 'Subtraction error';
  sBIZeroDivide        = 'Division by zero';
  sBIQuotientErr       = 'Quotient process error';
  sBIZeroFactor        = 'Factor is zero';
  sBIIterationCount    = 'Iterations must be more than 50';

  { ASN.1 conversion errors }
  sASNKeyTooLarge       = 'ASN key too large';
  sASNKeyBufferOverflow = 'Buffer OverFlow';
  sASNKeyBadModulus     = 'Asymmetric key modulus invalid';
  sASNKeyBadExponent    = 'Asymmetric key exponent invalid';
  sASNKeyBufferTooSmall = 'Buffer too small for key';
  sASNKeyBadKey         = 'Invalid Asymmetric Key';

  { RSA errors }
  sRSAKeyBadKey         = 'Invalid RSA Key';
  sModulusStringTooBig  = 'Modulus string too big';
  sExponentStringTooBig = 'Exponent string too big';
  sRSAKeyPairErr        = 'RSA key pair failure';
  sRSAPublicKeyErr      = 'Invalid RSA public key';
  sRSAPrivateKeyErr     = 'Invalid RSA private key';
  sRSAEncryptErr        = 'RSA encryption failure';
  sRSADecryptErr        = 'RSA decryption failure';
  sRSABlockSize128Err   = 'Invalid block size for key128';
  sRSABlockSize256Err   = 'Invalid block size for key256';
  sRSABlockSize512Err   = 'Invalid block size for key512';
  sRSABlockSize768Err   = 'Invalid block size for key768';
  sRSABlockSize1024Err  = 'Invalid block size for key1024';
  sRSABlockSize2048Err  = 'Invalid block size for key2048';
  sRSAEncodingErr       = 'RSA encoding error: too much data for block';
  sRSADecodingErrBTS    = 'RSA decoding error: Block too small';
  sRSADecodingErrBTL    = 'RSA decoding error: Block too large';
  sRSADecodingErrIBT    = 'RSA decoding error: Invalid block type';
  sRSADecodingErrIBF    = 'RSA decoding error: Invalid block format';

  { DSA errors }
  sDSAKeyBadKey         = 'Invalid DSA Key';
  sDSAParametersPQGErr  = 'DSA PQG parameter failure';
  sDSAParametersXYErr   = 'DSA XY parameter failure';
  sDSASignatureZeroR    = 'DSA Signature R is zero';
  sDSASignatureZeroS    = 'DSA Signature S is zero';
  sDSASignatureErr      = 'DSA Signature failure';

  { AboutBox errors }
  SNoStart = 'Unable to start web browser. Make sure you have it properly ' +
             'set-up on your system.';

implementation

end.
