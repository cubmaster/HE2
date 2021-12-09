using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Research.SEAL;

namespace HE2.ClientApp.Helpers
{

    public  class HeMath
    {    
                                                                  
                                                                                                                    
         private readonly KeyGenerator _keyGenerator;                                                                
         private Evaluator _evaluator;                                                                               
         private Encryptor _encryptor;                                                                               
         private PublicKey _public;                                                                                  
         private RelinKeys relinKeys;                                                                                
         private GaloisKeys _galoisKeys;                                                                             
         private CKKSEncoder _encoder;
         private SEALContext _context;
         private RelinKeys _relinKeys;
        public HeMath(SEALContext context,CKKSEncoder encoder, Encryptor encryptor,Evaluator evaluator,RelinKeys relinKeys)
        {
            _context = context;
            _encryptor = encryptor;
            _encoder = encoder;
            _evaluator = evaluator;
            _relinKeys = relinKeys;
        }
        public  Ciphertext Average(List<Ciphertext> ct)
        {
            var cresult = new Ciphertext();                                                  
            var presult = new Plaintext();
            var scale = Math.Pow(2.0,50);   
            _encoder.Encode(new List<double>((int)_encoder.SlotCount),scale,presult);  
            _encryptor.Encrypt(presult, cresult);

            var csum = Sum(ct);
            
            var factor = (double)1 / ct.Count;
            var ptFactor = new Plaintext();
            _encoder.Encode(factor,2^50,ptFactor);
            _evaluator.MultiplyPlain(csum,ptFactor,cresult);
            return cresult;

        }

        public  Ciphertext Sum( List<Ciphertext> ctTable)
        {
            var cresult = new Ciphertext();                                                  
            var presult = new Plaintext();
            var scale = Math.Pow(2.0,50);   
            _encoder.Encode(new List<double>((int)_encoder.SlotCount),scale,presult);                 
            _encryptor.Encrypt(presult, cresult);                                            
            for (int i = 0; i < ctTable.Count; i++)                                          
            {                                                                                
                _evaluator.Add(cresult,ctTable[i],cresult);                                  
            }

            return cresult;
        }
        
    }
}