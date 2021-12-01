using System;
using System.Diagnostics;
using System.IO;
using HE2.ClientApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Research.SEAL;
using System.Text.Json;
using System.Threading;

namespace HE2.Controllers
{   [ApiController]
    [Route("[controller]")]
    public class BFVController : Controller
    {
        
        private readonly SEALContext _sealContext;

        private readonly KeyGenerator _keyGenerator;
        private Evaluator _evaluator;
        private Encryptor _encryptor;
        private PublicKey _public;
        private RelinKeys relinKeys;
        
        public BFVController()
        {
            _sealContext = Utilities.GetContext();


            // Initialize key generator and encryptor
            // Initialize key Generator that will be use to get the Public and Secret keys
            _keyGenerator = new KeyGenerator(_sealContext);
            // Initializing encryptor
            _keyGenerator.CreatePublicKey(out _public);
            _encryptor = new Encryptor(_sealContext, _public);
            // Initialize evaluator
            _evaluator = new Evaluator(_sealContext);
            
            _keyGenerator.CreateRelinKeys(out RelinKeys rks);
            relinKeys = rks;
        }
        
        
        [HttpPost("Encrypt")]
        public IActionResult Encrypt([FromBody] int value)
        {
            var result = Utilities.CreateCiphertextFromInt(value, _encryptor);
            var artifact = new Artifact();
            artifact.key = Utilities.SecretKeyToBase64String(_keyGenerator.SecretKey);
            artifact.data =Utilities.CiphertextToBase64String(result);

            return Ok(artifact);


        }
        [HttpPost("Execute")]
        public IActionResult Execute([FromBody] Artifact artifact)
        {
            var ct = Utilities.BuildCiphertextFromBase64String(artifact.data, _sealContext);
            
            var result= new Ciphertext();
            
        
            _evaluator.Square(ct,result);
            Console.WriteLine($"    + size of result: {result.Size}");
          //  _evaluator.RelinearizeInplace(result, relinKeys);
            var resultArtifact = new Artifact();
            resultArtifact.data = Utilities.CiphertextToBase64String(result);

            return Ok(resultArtifact);

        }

        [HttpPost("decrypt")]
        public IActionResult decrypt([FromBody] Artifact artifact)
        {
            
            var ct =Utilities.BuildCiphertextFromBase64String(artifact.data,_sealContext);
            var key = Utilities.BuildSecretKeyFromBase64String(artifact.key, _sealContext);
            var decryptor = new Decryptor(_sealContext,key);
            Console.WriteLine($"Noise Budget Available: {decryptor.InvariantNoiseBudget(ct)}"); 
        
            
            var pt = new Plaintext();
            decryptor.Decrypt(ct,pt);
            Console.WriteLine($"Noise Budget Decrypted: {decryptor.InvariantNoiseBudget(ct)}"); 
            return Ok(Utilities.PlaintextToInt(pt));
            
        }

        [HttpPost("test")]
        public IActionResult test([FromBody] int value)
        {
         
            var result = Utilities.CreateCiphertextFromInt(value, _encryptor);
            
            var resultSq= new Ciphertext();
            _evaluator.SquareInplace(result);
            
            var decryptor = new Decryptor(_sealContext,_keyGenerator.SecretKey);
          
        
            
            var pt = new Plaintext();
            decryptor.Decrypt(result,pt);
            var answer = Utilities.PlaintextToInt(pt); 
            Console.WriteLine($"{value}^2={answer}");
            
            return Ok(answer);
        }
        
    }
    public  class Artifact
    {

   
        public string data { get; set; }
        public string key { get; set; }   
    }
   
}