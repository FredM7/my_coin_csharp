using MyCoin.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MyCoin
{
    public class BlockChain
    {
        private string _difficulty = "0000";


        public List<Block> Chain { get; set; } = new List<Block>();
        public List<Transaction> CurrentTransactions { get; set; } = new List<Transaction>();
        public List<string> Nodes { get; set; } = new List<string>();


        public BlockChain(string difficulty)
        {
            //Create a genesis block.
            NewBlock("100", "1");
        }


        /// <summary>
        /// Adds a new node to the list of nodes
        /// </summary>
        /// <param name="address">Address of node. Eg. http://localhost:5000</param>
        public void RegisterNode(string address)
        {
            //Never add dupes.
            if (!Nodes.Contains(address))
            {
                Nodes.Add(address);
            }
        }

        /// <summary>
        /// Determine if a given blockchain is valid
        /// </summary>
        /// <param name="blockChain">A blockchain</param>
        /// <returns>True if valid</returns>
        public bool ValidChain(List<Block> chain)
        {
            Block last_block = chain[0];
            int currentIndex = 1;

            while (currentIndex < chain.Count)
            {
                Block block = chain[currentIndex];
                //Check that the hash of the block is correct;
                if (block.PreviousHash != Hash(last_block))
                {
                    return false;
                }

                //Check that the Proof of Work is correct
                if (!ValidProof(last_block.Proof, block.Proof))
                {
                    return false;
                }

                last_block = block;
                currentIndex += 1;
            }

            return true;
        }

        /// <summary>
        /// This is our Consensus Algorithm, it resolves conflicts
        /// by replacing our chain with the longest one in the network.
        /// </summary>
        /// <returns>True if our chain was replaced</returns>
        public bool ResolveConflicts()
        {
            List<Block> newChain = null;
            List<string> neighbors = Nodes;
            long maxChainLength = Nodes.Count;
            //Grab and verify the chains from all the nodes in our network
            foreach (string node in neighbors)
            {
                var response = $"http://{node}/chain"; //TODO: Call service.
                if (response == "OK")
                {
                    List<Block> nodeChain = new List<Block>(); //TODO: use repsonse from serive!
                    long nodeChainLengh = 1; //TODO: use repsonse from serive!

                    if (nodeChainLengh > maxChainLength && ValidChain(nodeChain))
                    {
                        newChain = nodeChain;
                        maxChainLength = nodeChainLengh;
                    }
                }

                if (newChain != null)
                {
                    Chain = newChain;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a new Block and adds it to the chain
        /// </summary>
        /// <param name="proof"></param>
        /// <param name="previousHash"></param>
        public Block NewBlock(string proof, string previousHash)
        {
            Block block = new Block()
            {
                Index = Chain.Count + 1,
                PreviousHash = previousHash,
                Proof = proof,
                TimeStamp = DateTime.Now,
                Transactions = CurrentTransactions.ToList()
            };

            //Reset the current list of transactions
            CurrentTransactions.Clear();

            Chain.Add(block);

            return block;
        }

        /// <summary>
        /// Adds a new transaction to the list of transactions
        /// </summary>
        /// <param name="sender">Address of the Sender</param>
        /// <param name="recipient">Address of the Recipient</param>
        /// <param name="amount">Amount</param>
        public long NewTransaction(string sender, string recipient, decimal amount)
        {
            CurrentTransactions.Add(new Transaction()
            {
                Sender = sender,
                Recipient = recipient,
                Amount = amount
            });

            return LastBlock().Index + 1;
        }

        /// <summary>
        /// Creates a SHA-256 hash of a Block
        /// </summary>
        /// <param name="block">The block to hash</param>
        public string Hash(Block block)
        {
            SHA256Managed crypt = new SHA256Managed();
            StringBuilder hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(block)));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        /// <summary>
        /// Returns the last Block in the chain
        /// </summary>
        public Block LastBlock()
        {
            return Chain[Chain.Count - 1];
        }

        /// <summary>
        /// Simple Proof of Work Algorithm:
        /// - Find a number p' such that hash(pp') contains leading 4 zeroes, where p is the previous p'
        /// - p is the previous proof, and p' is the new proof
        /// </summary>
        /// <param name="lastProof"></param>
        public string ProofOfWork(string lastProof)
        {
            long proof = 0;
            while (!ValidProof(lastProof, proof.ToString()))
            {
                proof += 1;
            }
            return proof.ToString();
        }

        /// <summary>
        /// Validates the Proof: Does hash(last_proof, proof) contain 4 leading zeroes?
        /// </summary>
        /// <param name="lastProof">Previous Proof</param>
        /// <param name="proof">Current Proof</param>
        /// <returns></returns>
        private bool ValidProof(string lastProof, string proof)
        {
            string guess = JsonConvert.SerializeObject($"{lastProof}{proof}");

            string guessHash = Sha256(guess).Replace("-", "").ToLower();

            //Modify mining difficulty by increasing number of zeros. 
            return guessHash.StartsWith(_difficulty);
        }

        private string Sha256(string randomString)
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
