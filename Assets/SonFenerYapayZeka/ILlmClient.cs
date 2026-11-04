using System;
using System.Collections;
using System.Collections.Generic;

public interface ILlmClient
{
    IEnumerator GonderAsync(List<ChatMessage> mesajlar, Action<LlmSonuc> bitince);
}