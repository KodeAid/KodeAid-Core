// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.ComponentModel;

namespace KodeAid.ComponentModel
{
    public abstract class EditableObject : EditableObjectBase, IEditableObject
    {
        public new void BeginEdit()
        {
            base.BeginEdit();
        }

        public new void CancelEdit()
        {
            base.CancelEdit();
        }

        public new void EndEdit()
        {
            base.EndEdit();
        }
    }
}
