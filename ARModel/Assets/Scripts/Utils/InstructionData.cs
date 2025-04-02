using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils { 

    /// <summary>
    /// Handles the instruction content and provides a specific one.
    /// </summary>
    public class InstructionData
    {
        #region fields
    
        // define the necessary instructions
        private string[] instructions = new string[]
        {
            "Point your camera to a surface to detect an AR plane.",
            "An AR plane can have different colors: Orange for vertical plane, Blue for floor plane and " +
            "purple for other horizontal surfaces.",
            "When an AR plane is detected, click on it to add an Object.",
            "Click on top of an added object and use your fingers to move, rotate and scale it.",
            "Use the object menu to delete it and to record a voice command like \"rotate\", \"forward\" and \"scale\".",
            "You can add more objects by clicking in AR planes. Be aware that the objects you have, " +
            "the more performance constraints will exist.",
            "Use the menu bar to to toggle AR Notes, see the AR planes, select other object to add, " +
            "activate physics engine between the objects.",
            "If you need to open these instructions again, click in the instructions button."
        };

        private int currentInstructionIndex = 0;

        #endregion

        #region properties

        /// <summary>
        /// Returns the current instruction index
        /// </summary>
        /// <returns></returns>
        public int CurrentInstructionIndex
        {
            get{ return currentInstructionIndex; }
        }

        public bool IsLastInstruction
        {
            get { return currentInstructionIndex == (instructions.Length - 1); }
        }

        public bool IsFirstInstruction
        {
            get { return currentInstructionIndex == 0; }
        }

        #endregion


        #region public methods

        /// <summary>
        /// Returns the current instruction
        /// </summary>
        /// <returns></returns>
        public string GetCurrentInstruction()
        { 
            return instructions[currentInstructionIndex]; 
        }

        /// <summary>
        /// Returns the following instruction
        /// </summary>
        /// <returns></returns>
        public string GetforwardInstruction()
        {
            if (currentInstructionIndex < instructions.Length - 1) 
            {
                currentInstructionIndex++;
            }
        
            return instructions[currentInstructionIndex]; 
        }

        /// <summary>
        /// Returns previous instruction
        /// </summary>
        /// <returns></returns>
        public string GetPreviousInstruction()
        {
            if (currentInstructionIndex > 0)
            {
                currentInstructionIndex--;
            }
        
            return instructions[currentInstructionIndex];
        }

        /// <summary>
        /// Resets the instructions order
        /// </summary>
        public void ResetInstructions()
        {
            currentInstructionIndex = 0;
        }

        #endregion

    }
}