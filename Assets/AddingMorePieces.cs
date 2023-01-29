//after creating a new piece do not forget to
//1. make a new tag for it and change its the piece's tag to the new tag
//2. go to the TeamBuilderManager and add it to the dictionary
//3. add the object into the array of monsterPieces[] in GameManager
//4. in GameObject GetMonsterFromTag in the _GameManagerScript
//4a. ad the if(tag == new tag for the new monster)
//4b. in the if function instantiation make sure in the array its instantiating that monster