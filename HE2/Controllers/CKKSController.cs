using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using HE2.ClientApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Research.SEAL;

namespace HE2.Controllers
{   [ApiController]
    [Route("[controller]")]
    public class CKKSController : Controller
    {
        private readonly SEALContext _sealContext;

        private readonly KeyGenerator _keyGenerator;
        private Evaluator _evaluator;
        private Encryptor _encryptor;
        private PublicKey _public;
        private RelinKeys relinKeys;
        private GaloisKeys _galoisKeys;
        private CKKSEncoder _encoder;
        public CKKSController()
        {
            
            _sealContext = Utilities.GetContext(SchemeType.CKKS);
            // Initialize key generator and encryptor
            // Initialize key Generator that will be use to get the Public and Secret keys
            _keyGenerator = new KeyGenerator(_sealContext);
            // Initializing encryptor
            _keyGenerator.CreatePublicKey(out _public);
            _encryptor = new Encryptor(_sealContext, _public);
            // Initialize evaluator
            _evaluator = new Evaluator(_sealContext); 
            _encoder = new CKKSEncoder(_sealContext);
            
            _keyGenerator.CreateRelinKeys(out RelinKeys rks);
            relinKeys = rks;
            _keyGenerator.CreateGaloisKeys(out GaloisKeys gks);
            _galoisKeys = gks;
        }
        [HttpGet("test")]
        public IActionResult Test()
        {
           var slotCount = _encoder.SlotCount;
         
           var table = new List<List<double>>();
           var scale = Math.Pow(2.0,50);
           var ctTable = new List<Ciphertext>();
           double currPoint = 0, stepSize = 1.0 / (slotCount - 1); 


      var rand = new Random();
      for (int r = 0; r < 5; r++)
      {           
          var row = new List<double>((int)slotCount);   
          for (int c = 0; c < 50; c++)
          {
                
                row.Add(2);
          } 
          table.Add(row);
      }
      
      
      
      foreach (var row in table)
      {
          var pt = new Plaintext();
          var ct = new Ciphertext();
          _encoder.Encode(row, scale, pt);
          _encryptor.Encrypt(pt, ct);
          ctTable.Add(ct);
      }


      var heMath = new HeMath(_sealContext,_encoder,_encryptor,_evaluator,relinKeys);
     
      var c_sum = heMath.Sum(ctTable);
      var c_avg = heMath.Average(ctTable);

      
      using Decryptor decryptor = new Decryptor(_sealContext, _keyGenerator.SecretKey);
     //Decrypt sums
               Plaintext p_sum = new Plaintext();
             
               decryptor.Decrypt(c_sum, p_sum);
               var sum_result = new List<double>();
               _encoder.Decode(p_sum, sum_result);
              // Console.WriteLine(JsonSerializer.Serialize(result));
               Utilities.PrintVector(sum_result, 10, 7);
           
            
        //Decrypt Averages    
        Plaintext p_avg = new Plaintext();
        
        decryptor.Decrypt(c_avg, p_avg);
        var avg_result = new List<double>();
        _encoder.Decode(p_avg, avg_result);
        // Console.WriteLine(JsonSerializer.Serialize(result));
        Utilities.PrintVector(avg_result, 10, 7);
 
            return Ok(0);
        }
    }
}