using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteScript : MonoBehaviour
{
   private bool noteStatus;
   public GameObject note;

   public void ToggleNote()
   {
    noteStatus = !noteStatus;
    note.SetActive(noteStatus);
   }

   public bool GetNoteStatus()
   {
    return noteStatus;
   }


}
