using MyCoin.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MyCoin
{
    class Program
    {
        static Guid _nodeIdentifier = Guid.NewGuid();
        static BlockChain _blockchain = new BlockChain("777");

        static void Main(string[] args)
        {
            //Register this node example...
            string result = RegisterNodes(new List<string>()
            {
                "http://localhost:5000"
            });
            
            //Mine a certain amount of blocks.
            for (int i = 0; i < 20; i++)
            {
                Mine();
            }
        }

        private static string Mine()
        {
            Block lastBlock = _blockchain.LastBlock();
            string lastProof = lastBlock.Proof;
            string proof = _blockchain.ProofOfWork(lastProof);

            //Reward for finding proof.
            _blockchain.NewTransaction("0", _nodeIdentifier.ToString(), 1);

            //Forge the new Block by adding it to the chain.
            string previousHash = _blockchain.Hash(lastBlock);
            Block block = _blockchain.NewBlock(proof, previousHash);

            ResponseTwo response = new ResponseTwo
            {
                Message = "New block forged",
                Index = block.Index,
                Transactions = block.Transactions,
                Proof = block.Proof,
                PreviousHash = block.PreviousHash
            };

            return JsonConvert.SerializeObject(response);
        }

        private static string RegisterNodes(List<string> nodes)
        {
            if (nodes != null && nodes.Count > 0)
            {
                foreach (string node in nodes)
                {
                    _blockchain.RegisterNode(node);
                }
                return "New nodes have been added.";
            }
            else
            {
                return "Please supply a valid list of nodes!";
            }
        }

        private void Consensus()
        {
            bool replaced = _blockchain.ResolveConflicts();

            if (replaced)
            {
                //Our chain was replaced!
            }
            else
            {
                //Our chain is authorative!
            }
        }

        private string FullChain()
        {
            FullChain fullChain = new FullChain
            {
                Chain = _blockchain.Chain
            };

            return JsonConvert.SerializeObject(fullChain);
        }

        private Response NewTransaction(string sender, string recipient, decimal amount)
        {
            long index = _blockchain.NewTransaction(sender, recipient, amount);
            Response response = new Response
            {
                Message = $"Transaction will be added to Block {index}"
            };

            return response;
        }
    }
}
