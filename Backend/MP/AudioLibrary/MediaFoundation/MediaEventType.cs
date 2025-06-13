
using System.Runtime.Versioning;

namespace MP.AudioLibrary.MediaFoundation
{
    public enum MediaEventType : System.UInt32
    {
        //-------------------------------------------------------------------------
        // generic events
        //-------------------------------------------------------------------------

        /// <member name="MEUnknown">
        ///     Unknown event type.  Do not use for legitimate events.
        /// </member>
        MEUnknown = 0,

        /// <member name="MEError">
        ///     Any component can send MEError event at any time to notify
        ///     about a fatal failure.
        /// </member>
        MEError = 1,

        /// <member name="MEExtendedType">
        ///     Extensible event type.  Use the extended event type
        ///     (IMFMediaEvent::GetExtendedType)
        /// </member>
        MEExtendedType = 2,

        /// <member name="MENonFatalError">
        ///     Any component can send MENonFatalError event at any time to notify
        ///     about a non-fatal error.
        /// </member>
        MENonFatalError = 3,

        /// <member name="MEGenericV1Anchor">
        ///     Last generic event for v1.  Do not add or remove events above this one.
        /// </member>
        MEGenericV1Anchor = MENonFatalError,

        //-------------------------------------------------------------------------
        // Media Session events:
        // Events of interest to applications using the Media Session
        //-------------------------------------------------------------------------


        /// <member name="MESessionUnknown">
        ///     Unknown event type.  Do not use for legitimate events.
        /// </member>
        MESessionUnknown = 100,

        /// <member name="MESessionTopologySet">
        ///     Media Session response to IMFMediaSession::SetTopology.
        ///     Value:
        ///         VT_UNKNOWN
        ///         IMFTopology * for the full topology that was set
        /// </member>
        MESessionTopologySet = 101,

        /// <member name="MESessionTopologiesCleared">
        ///     Media Session response to IMFMediaSession::ClearTopologies
        /// </member>
        MESessionTopologiesCleared = 102,

        /// <member name="MESessionStarted">
        ///     Media Session response to IMFMediaSession::Start
        ///  
        ///     MF_EVENT_PRESENTATION_TIME_OFFSET:
        ///         Indicates the offset between presentation time and original
        ///         content timestamps (source time) for this presentation.
        ///         Applications may prefer to display source time
        ///         for their UI and should use this value to do so.
        ///         The relationship between presentation time and source time is:
        ///             PresentationTimeOffset = PresentationTime - SourceTime
        ///
        /// </member>
        MESessionStarted = 103,

        /// <member name="MESessionPaused">
        ///     Media Session response to IMFMediaSession::Pause
        /// </member>
        MESessionPaused = 104,

        /// <member name="MESessionStopped">
        ///     Media Session response to IMFMediaSession::Stop
        /// </member>
        MESessionStopped = 105,

        /// <member name="MESessionClosed">
        ///     Media Session response to IMFMediaSession::Close
        /// </member>
        MESessionClosed = 106,

        /// <member name="MESessionEnded">
        ///     Media Session sends when it has reached the end of
        ///     the presentation(s).  Pipeline will then enter the
        ///     stopped state
        /// </member>
        MESessionEnded = 107,

        /// <member name="MESessionRateChanged">
        ///     Media Session response to IMFRateControl::SetRate on its
        ///     rate control service
        ///     Value:
        ///         VT_R4
        ///         New rate
        /// </member>
        MESessionRateChanged = 108,

        /// <member name="MESessionScrubSampleComplete">
        ///     Media Session response to IMFMediaSession::Start when the
        ///     rate is set to 0 ("scrubbing").  This means that a frame
        ///     for the specified time has been displayed
        /// </member>
        MESessionScrubSampleComplete = 109,

        /// <member name="MESessionCapabilitiesChanged">
        ///     Media Session sends this events whenever there is an update to
        ///     its capabilities
        ///     Attributes:
        ///     MF_EVENT_SESSIONCAPS:
        ///         Combination of MFSESSIONCAP_xxx bitflags
        ///     MF_EVENT_SESSIONCAPS_DELTA:
        ///         Delta between the previous session capabilities and the
        ///         current ones
        /// </member>
        MESessionCapabilitiesChanged = 110,

        /// <member name="MESessionTopologyStatus">
        ///     Media Session sends this informational event to report the 
        ///     status of the associated topology to the application
        ///     Value:
        ///         VT_UNKNOWN
        ///         IMFTopology * for the full topology whose status this is
        ///     Attributes:
        ///     MF_EVENT_TOPOLOGY_STATUS:
        ///         Status of this topology.  See the MF_TOPOSTATUS enumerated
        ///         type values for details.
        /// </member>
        MESessionTopologyStatus = 111,

        /// <member name="MESessionNotifyPresentationTime">
        ///     This event is sent by the Media Session whenever a a new topology
        ///     is started.  It indicates when the presentation will start
        ///     and the offset between presentation time and the original content's
        ///     timestamps.
        ///     Attributes:
        ///     MF_EVENT_START_PRESENTATION_TIME:
        ///         Time (in terms of Presentation Clock time) this topology
        ///         will start
        ///     MF_EVENT_START_PRESENTATION_TIME_AT_OUTPUT
        ///         Indicates presentation time when sink is going to render the first sample of new topology
        ///         If any nodes in topology are buffering, MF_EVENT_OUTPUT_PRESENTATION_TIME will be smaller then
        ///         MF_EVENT_PRESENTATION_TIME
        ///     MF_EVENT_PRESENTATION_TIME_OFFSET:
        ///         Indicates the offset between presentation time and original
        ///         content timestamps (source time) for this presentation.
        ///         Applications may prefer to display source time
        ///         for their UI and should use this value to do so.
        ///         The relationship between presentation time and source time is:
        ///             PresentationTimeOffset = PresentationTime - SourceTime
        /// </member>
        MESessionNotifyPresentationTime = 112,

        /// <member name="MENewPresentation">
        ///     This event originates from a multiple-presentations Media Source and
        ///     is forwarded to the application by the Media Session.
        ///     Application can obtain the corresponding topology from
        ///     the IMFMediaSourceTopologyProvider interface on the Media Source
        ///     and queue this topology at the Media Session via
        ///     IMFMediaSession::SetTopology.
        ///     Value: 
        ///         VT_UNKNOWN
        ///             IMFPresentationDescriptor * for the new presentation. 
        ///     Attributes:
        ///     There are no attributes corresponding to the MENewPresentation 
        ///     event; however, the following attribute may be present on 
        ///     the IMFPresentationDescriptor * associated with this event.
        ///     MF_PD_PLAYBACK_ELEMENT_ID:
        ///         This is an optional attribute for Media Sources with playlist-
        ///         style content.  Its value indicates which playlist element
        ///         this presentation corresponds.
        ///         When the application uses this PresentationDescriptor to build
        ///         a new topology, it should transfer this attribute value to
        ///         the topology that it builds.
        ///         Example: Suppose the Media Source is sourcing Element1, 
        ///         Element2, Element3 in sequence.  The Presentation Descriptors 
        ///         for the MENewPresentation events announcing each of these 
        ///         should have different values for this attribute.
        ///         However, if the Media Source is firing 
        ///         MENewPresentation to announce a presentation change on a 
        ///         particular element, such as some kind of dynamic stream switch 
        ///         or addition/removal, this value should remain the same to 
        ///         indicate that it is the same actual element.
        ///         Note that this is very similar in intent to the IMFTopologyNode
        ///         attribute MF_TOPONODE_SEQUENCE_ELEMENTID.  Media Sources that
        ///         provide topologies (i.e. that implement 
        ///         IMFMediaSourceTopologyProvider) should use that mechanism
        ///         instead, since, unlike this attribute, it will be recognized 
        ///         by MF on Windows Vista.
        ///         If two consecutive presentations have the same value for this
        ///         attribute, the MF pipeline will expect their timestamps to
        ///         remain continuous across the transition; therefore, the
        ///         Media Source should _not_ use the MF_EVENT_SOURCE_ACTUAL_START
        ///         when rolling to the next presentation.
        ///         Media Sources that set this attribute on the 
        ///         IMFPresentationDescriptors accompanying MENewPresentation events
        ///         should also set it on the IMFPresentationDescriptor produced
        ///         by IMFMediaSource::CreatePresentationDescriptor.
        ///     MF_PD_PLAYBACK_BOUNDARY_TIME:
        ///         This is an optional attribute for Media Sources with playlist-
        ///         style content.  Its value indicates where (in Media Source time)
        ///         this presentation actually starts.  
        ///         When the application uses this PresentationDescriptor to build
        ///         a new topology, it should transfer this attribute value to
        ///         the topology that it builds.
        ///         Example: Suppose that the Media Source is sourcing Element1,
        ///         Element2, Element3 in sequence.  In the middle of Element1, 
        ///         there is a dynamic stream change (or addition/removal) at t=15.
        ///         However, in the new stream, the nearest previous keyframe is
        ///         at t=12, and therefore the Media Source will start sourcing
        ///         this stream from t=12 after the transition.  What we want here
        ///         is a markin that will drop the decoded samples in [12, 15).
        ///         The MENewPresentation event for this change, in addition to
        ///         carrying a value MF_PD_PLAYBACK_ELEMENT_ID=1 as described
        ///         above, would also carry MF_PD_BOUNDARY_TIME=150000000, to 
        ///         indicate that this happens at t=15.  The Media Processor will
        ///         perform a markin at t=15 (usually right after decoding), and 
        ///         this will prevent the t=12...15 frames from being displayed.
        ///         Note that this is very similar in intent to the IMFTopologyNode
        ///         attribute MF_TOPONODE_MEDIASTART.  Media Sources that provide
        ///         topologies (i.e. that implement IMFMediaSourceTopologyProvider)
        ///         should use that mechanism instead, since, unlike this attribute,
        ///         it will be recognized by MF on Windows Vista.
        ///         This value does not affect how the MF pipeline adjusts 
        ///         timestamps; it affects only markin time.
        ///         This attribute will be ignored unless the Media Source 
        ///         indicates, by means of the MF_PD_PLAYBACK_ELEMENT_ID attribute
        ///         described above, that this presentation is the same playback
        ///         element as the previous one.
        /// </member>
        MENewPresentation = 113,

        /// <member name="MELicenseAcquisitionStart">
        ///     Indicates that license acquisition is about to begin.
        ///     License acquisition is done through the application-provided
        ///     IMFContentProtectionManager
        /// </member>
        MELicenseAcquisitionStart = 114,

        /// <member name="MELicenseAcquisitionCompleted">
        ///     Indicates that license acquisition is complete
        /// </member>
        MELicenseAcquisitionCompleted = 115,

        /// <member name="MEIndividualizationStart">
        ///     Indicates that individualization is about to begin.
        ///     Individualization is done through the application-provided
        ///     IMFContentProtectionManager
        /// </member>
        MEIndividualizationStart = 116,

        /// <member name="MEIndividualizationCompleted">
        ///     Indicates that individualization is complete
        /// </member>
        MEIndividualizationCompleted = 117,

        /// <member name="MEEnablerProgress">
        ///     Optionally sent by the Input Trust Authority's implementation of
        ///     IMFContentEnabler to the application-provided
        ///     IMFContentProtectionManager.
        ///     Value:
        ///         VT_LPWSTR
        ///         Progress update for the Content Enabler's current action
        /// </member>
        MEEnablerProgress = 118,

        /// <member name="MEEnablerCompleted">
        ///     Optionally sent by the Input Trust Authority's implementation of
        ///     IMFContentEnabler to the application-provided
        ///     IMFContentProtectionManager.
        /// </member>
        MEEnablerCompleted = 119,

        /// <member name="MEPolicyError">
        ///     Sent by any trusted component when an error is encountered enforcing
        ///     the required content protection policy.
        ///     This event is forwarded to the application by the Media Session,
        ///     and the MF pipeline stops playback.
        /// </member>
        MEPolicyError = 120,

        /// <member name="MEPolicyReport">
        ///     Sent by trusted outputs to report on how the policy was applied.
        ///     Attributes and status codes are specific to the content
        ///     protection system being used.
        /// </member>
        MEPolicyReport = 121,

        /// <member name="MEBufferingStarted">
        ///     Indicates that the MF pipeline is pausing in order to buffer
        ///     data as needed by one or more of its Media Sources.
        /// </member>
        MEBufferingStarted = 122,

        /// <member name="MEBufferingStopped">
        ///     Indicates that the MF pipeline is done buffering and will start
        ///     playback
        /// </member>
        MEBufferingStopped = 123,

        /// <member name="MEConnectStart">
        ///     If the application provides an IMFSourceOpenMonitor callback,
        ///     this event is delivered directly to that callback when the
        ///     MF network source starts connecting to the media
        /// </member>
        MEConnectStart = 124,

        /// <member name="MEConnectEnd">
        ///     If the application provides an IMFSourceOpenMonitor callback,
        ///     this event is delivered directly to that callback when the
        ///     MF network source is done connecting to the media
        /// </member>
        MEConnectEnd = 125,

        /// <member name="MEReconnectStart">
        ///     Indicates that the Media Source is attempting to reconnect
        ///     to the media
        /// </member>
        MEReconnectStart = 126,

        /// <member name="MEReconnectStop">
        ///     Indicates that the Media Source has finished reconnecting to
        ///     the media
        /// </member>
        MEReconnectEnd = 127,


        /// <member name="MERendererEvent">
        ///     This event can be generated by a custom renderer.
        ///     <para>
        ///         Value: VT_I4
        ///         This is the custom event code generated by the renderer.
        ///     </para>
        /// </member>
        MERendererEvent = 128,

        /// <member name="MESessionStreamSinkFormatChanged">
        ///     This event is generated by the Media Session when a stream
        ///     sink's format has changed.
        ///     Attributes:
        ///     MF_EVENT_OUTPUT_NODE
        ///         The topology node id of the stream sink whose format changed.
        /// </member>
        MESessionStreamSinkFormatChanged = 129,

        /// <member name="MESessionV1Anchor">
        ///     Last session event for v1.  Do not add or remove events above this one.
        /// </member>
        MESessionV1Anchor = MESessionStreamSinkFormatChanged,

        //-------------------------------------------------------------------------
        // Media Source events:
        // Events of interest to Media Source implementations or applications
        // using Media Sources directly
        //-------------------------------------------------------------------------

        /// <member name="MESourceUnknown">
        ///     Unknown event type.  Do not use for legitimate events.
        /// </member>
        MESourceUnknown = 200,

        /// <member name="MESourceStarted">
        ///     Media Source response to IMFMediaSource::Start for non-seeking
        ///     starts.
        ///     Value:
        ///         VT_I8
        ///         MFTIME for start time, relative to sample timestamps.
        ///         Can be empty if the start is from the current position
        ///         (i.e. the start time given to IMFMediaSource::Start was
        ///         VT_EMPTY), and the Media Source is either already in the
        ///         started state or is resuming playback from the paused state.
        ///         Otherwise, this must be set to a valid value.
        ///     Attributes:
        ///         MF_EVENT_SOURCE_FAKE_START
        ///             This attribute should be set by Media Sources that work
        ///             with topologies (such as the MF Sequencer Source).
        ///             That Media Source should set this attribute to a nonzero
        ///             value if the topology currently being started will be
        ///             empty (i.e. playback will not actually start from this
        ///             topology)
        ///         MF_EVENT_SOURCE_PROJECTSTART
        ///             This attribute should be set by Media Sources that work
        ///             with topologies (such as the MF Sequencer Source).
        ///             If the topology from which we will be starting has an 
        ///             MF_TOPOLOGY_PROJECTSTART attribute, then that Media Source 
        ///             should set this attribute on the event to the same value.
        ///         MF_EVENT_SOURCE_ACTUAL_START
        ///             This attribute is relevant only if the value associated 
        ///             with this MESourceStarted event is VT_EMPTY.
        ///             Its value indicates the time (in terms of source time)
        ///             at which this Media Source is started.
        ///             For example, suppose a live Media Source is started at 
        ///             "current position", and its timestamps will start at some
        ///             unknown "current" point.  The MESourceStarted event will
        ///             carry a VT_EMPTY value, since that is what was requested in
        ///             IMFMediaSource::Start, but this attribute will indicate 
        ///             where this Media Source is actually starting.
        ///             If this attribute is absent and the MESourceStarted event
        ///             carries a VT_EMPTY value, then this attribute's value
        ///             should be assumed to be zero.
        /// </member>
        MESourceStarted = 201,

        /// <member name="MEStreamStarted">
        ///     Each Media Stream sends this event in response to
        ///     IMFMediaSource::Start for a non-seeking start
        ///     Value:
        ///         VT_I8
        ///         See description for MESourceStarted above
        /// </member>
        MEStreamStarted = 202,

        /// <member name="MESourceSeeked">
        ///     Media Source response to IMFMediaSource::Start for seeking starts.
        ///     A start is considered to be a "seek" if the playback position
        ///     is being changed and the Media Source is in the playing state
        ///     or is resuming from the paused state.
        ///     Value:
        ///         VT_I8
        ///         See description above for MESourceStarted event
        ///     Attributes:
        ///         See description above for MESourceStarted event
        /// </member>
        MESourceSeeked = 203,

        /// <member name="MEStreamSeeked">
        ///     Each Media Stream sends this event in response to
        ///     IMFMediaSource::Start for a seeking start
        ///     Value:
        ///         VT_I8
        ///         See description for MESourceStarted above
        /// </member>
        MEStreamSeeked = 204,

        /// <member name="MENewStream">
        ///     Media Source response to IMFMediaSource::Start.
        ///     One such event is sent for each stream that is appearing for the
        ///     first time in this start.
        ///     Value:
        ///         VT_UNKNOWN
        ///         IMFMediaStream * of new stream
        /// </member>
        MENewStream = 205,

        /// <member name="MEUpdatedStream">
        ///     Media Source response to IMFMediaSource::Start.
        ///     One such event is sent for each stream in this start that has
        ///     appeared in a previous start.
        ///     Value:
        ///         VT_UNKNOWN
        ///         IMFMediaStream * of stream
        /// </member>
        MEUpdatedStream = 206,

        /// <member name="MESourceStopped">
        ///     Media Source response to IMFMediaSource::Stop
        /// </member>
        MESourceStopped = 207,

        /// <member name="MEStreamStopped">
        ///     Each Media Stream sends this event in response to
        ///     IMFMediaSource::Stop
        /// </member>
        MEStreamStopped = 208,

        /// <member name="MESourcePaused">
        ///     Media Source response to IMFMediaSource::Pause
        /// </member>
        MESourcePaused = 209,

        /// <member name="MEStreamPaused">
        ///     Each Media Stream sends this event in response to
        ///     IMFMediaSource::Pause
        /// </member>
        MEStreamPaused = 210,

        /// <member name="MEEndOfPresentation">
        ///     This event originates from the Media Source and is forwarded
        ///     to the application by the Media Session.
        ///     It means that the final (or only) segment is finished.
        ///     No action required from the application
        /// </member>
        MEEndOfPresentation = 211,

        /// <member name="MEEndOfStream">
        ///     Each Media Stream sends this event when it hits the end of
        ///     the stream.
        ///     The Media Streams of the sequencer Media Source will send
        ///     this event at the end of each presentation.
        /// </member>
        MEEndOfStream = 212,

        /// <member name="MEMediaSample">
        ///     Sent by the Media Stream to deliver a new sample
        ///     Value:
        ///         VT_UNKNOWN
        ///         IMFSample * for the sample
        /// </member>
        MEMediaSample = 213,

        /// <member name="MEStreamTick">
        ///     Sent by a Media Stream to indicate that there will be no
        ///     samples delivered on this stream until at least the indicated
        ///     time.
        ///     Value:
        ///         VT_I8
        ///         MFTIME (relative to sample timestamps) until which
        ///         the Media Stream expects not to deliver any data
        /// </member>
        MEStreamTick = 214,

        /// <member name="MEStreamThinMode">
        ///     Sent to indicate that the stream is being thinned. A thinned stream
        ///     delivers some samples but not all of them. An example
        ///     is when a media source is only delivering key frames.

        ///
        ///     Value:
        ///         VT_BOOL
        ///         VARIANT_TRUE -- the following samples will be thinned
        ///         VARIANT_FALSE -- the following samples are not being thinned
        /// </member>
        MEStreamThinMode = 215,

        /// <member name="MEStreamFormatChanged">
        ///     Sent by a Media Stream indicating a format change in the media stream.  
        ///     Value:
        ///         VT_UNKNOWN
        ///         IMFMediaType * for the new media type for the stream
        /// </member>
        MEStreamFormatChanged = 216,

        /// <member name="MESourceRateChanged">
        ///     Media Source response to IMFRateControl::SetRate on its
        ///     rate control service, if it exists
        ///     Value:
        ///         VT_R4
        ///         New rate
        /// </member>
        MESourceRateChanged = 217,

        //
        // MENewPresentation
        // Sent by the sequencer Media Source to notify the application of the
        // next presentation in the sequence.
        // This event is described in the "Media Session events" section above.
        //

        /// <member name="MEEndOfPresentationSegment">
        ///     This event originates from a sequencer Media Source and is
        ///     forwarded to the application by the Media Session.
        ///     It means that the sequencer has finished a non-final segment and
        ///     has moved onto the next one.
        ///     No action required from the application.
        ///     Attributes:
        ///         MF_EVENT_SOURCE_TOPOLOGY_CANCELED
        ///             If present and nonzero, that means that the presentation
        ///             has ended because the Media Source has canceled this
        ///             topology
        /// </member>
        MEEndOfPresentationSegment = 218,

        /// <member name="MESourceCharacteristicsChanged">
        ///     Sent by a Media Source if its characteristics change.
        ///     Attributes:
        ///     MF_EVENT_SOURCE_CHARACTERISTICS
        ///         New IMFMediaSource characteristics bitflags
        ///     MF_EVENT_SOURCE_CHARACTERISTICS_OLD
        ///         Previous IMFMediaSource characteristics bitflags
        /// </member>
        MESourceCharacteristicsChanged = 219,

        /// <member name="MESourceRateChangeRequested">
        ///     Sent by a Media Source if it requires that the rate be changed
        ///     A typical reason for sending this event would be if the Media
        ///     Source cannot continue playback at the current rate.
        ///     Value:
        ///         VT_R4
        ///         New rate requested by the Media Source
        ///     Attributes:
        ///         MF_EVENT_DO_THINNING
        ///             If present and non-zero, this event is requesting
        ///             that thinning at the new rate
        /// </member>
        MESourceRateChangeRequested = 220,

        /// <member name="MESourceMetadataChanged">
        ///     Sent by a Media Source if the metadata has been updated.
        ///     Usually this happens when the Media Source can not provide
        ///     all the metadata at startup.
        /// </member>
        MESourceMetadataChanged = 221,

        /// <member name="MESequencerSourceTopologyUpdated">
        ///     This event is sent in response to the asynchronous method 
        ///     IMFSequncerSource::UpdateTopology. This is just a notification to 
        ///     the application that the sequencer source is handling the 
        ///     UpdateTopology request. The application should use the
        ///     MESessionTopologyXXX events to see when the topology is actually
        ///     ready for playback.
        ///     <para>
        ///         Value: VT_UI4
        ///         The sequencer ID of the topology which was updated.
        ///     </para>
        /// </member>
        MESequencerSourceTopologyUpdated = 222,

        /// <member name="MESourceV1Anchor">
        ///     Last source event for v1.  Do not add or remove events above this one.
        /// </member>
        MESourceV1Anchor = MESequencerSourceTopologyUpdated,

        //
        // MEBufferingStarted
        // Sent by Media Sources that need to buffer data while starting.
        // The MF pipeline will pause until it receives MEBufferingStopped.
        // This event is described in the "Media Session events" section above
        //

        //
        // MEBufferingStopped
        // Sent by Media Sources that need to buffer data while starting.
        // The MF pipeline will enter the started state and begin playing.
        // This event is described in the "Media Session events" section above
        //

        //
        // MEReconnectStart
        // Sent by a Media Source when it attempts to reconnect to the media
        // This event is described in the "Media Session events" section above
        //

        //
        // MEReconnectEnd
        // Sent by a Media Source when it is done reconnecting to the media
        // This event is described in the "Media Session events" section above
        //

        //-------------------------------------------------------------------------
        // Media Sink events:
        // Events of interest to IMFMediaSink implementations or applications
        // that use Media Sinks directly
        //-------------------------------------------------------------------------

        /// <member name="MESinkUnknown">
        ///     Unknown event type.  Do not use for legitimate events.
        /// </member>
        MESinkUnknown = 300,

        /// <member name="MEStreamSinkStarted">
        ///     Each Stream Sink sends this event in response to the
        ///     IMFClockStateSink::OnClockStart call made on the Media Sink
        /// </member>
        MEStreamSinkStarted = 301,

        /// <member name="MEStreamSinkStopped">
        ///     Each Stream Sink sends this event in response to the
        ///     IMFClockStateSink::OnClockStop call made on the Media Sink
        /// </member>
        MEStreamSinkStopped = 302,

        /// <member name="MEStreamSinkPaused">
        ///     Each Stream Sink sends this event in response to the
        ///     IMFClockStateSink::OnClockPause call made on the Media Sink
        /// </member>
        MEStreamSinkPaused = 303,

        /// <member name="MEStreamSinkRateChanged">
        ///     Each Stream Sink sends this event in response to the
        ///     IMFClockStateSink::OnClockRateChanged call made on the Media Sink
        /// </member>
        MEStreamSinkRateChanged = 304,

        /// <member name="MEStreamSinkRequestSample">
        ///     A Stream Sink sends this event to request for each sample
        ///     that it wants delivered to IMFStreamSink::ProcessSample
        /// </member>
        MEStreamSinkRequestSample = 305,

        /// <member name="MEStreamSinkMarker">
        ///     A Stream Sink sends this event in response to
        ///     IMFStreamSink::PlaceMarker, when it hits that marker.
        ///     Markers should be processed in order with samples.
        ///     Value:
        ///         [Type determined by IMFStreamSink::PlaceMarker call]
        ///         Copy of the PROPVARIANT passed into IMFStreamSink::PlaceMarker
        ///         as the "context" for the marker
        /// </member>
        MEStreamSinkMarker = 306,

        /// <member name="MEStreamSinkPrerolled">
        ///     Each Stream Sink of a Media Sink that supports IMFMediaSinkPreroll
        ///     sends this event when enough samples have been received through
        ///     IMFStreamSinkProcessSample to preroll up to the time specified
        ///     in IMFMediaSinkPreroll::NotifyPreroll
        /// </member>
        MEStreamSinkPrerolled = 307,

        /// <member name="MEStreamSinkScrubSampleComplete">
        ///     Each Stream Sink of a Media Sink that supports rate 0 sends
        ///     this event when the rate is 0, as soon as a frame has been
        ///     displayed for the time specified in IMFClockStateSink::OnClockStart.
        ///     Sinks that support rate 0 but do not render anything will send
        ///     back this event as soon as they receive the OnClockStart 
        ///     notification.
        ///     Attributes:
        ///         MF_EVENT_SCRUBSAMPLE_TIME
        ///             If present, indicates the presentation time for which
        ///             a frame has been displayed.
        ///             Sinks that do not render any data while in rate 0 should
        ///             not set this attribute.
        /// </member>
        MEStreamSinkScrubSampleComplete = 308,

        /// <member name="MEStreamSinkFormatChanged">
        ///     Sent by a Stream Sink when the downstream format changes in such
        ///     a way that will require the topology to be renegotiated.
        /// </member>
        MEStreamSinkFormatChanged = 309,

        /// <member name="MEStreamSinkDeviceChanged">
        ///     Sent by the Stream Sink of the Enhanced Video Renderer to
        ///     indicate that the device has changed.  The MF pipeline responds
        ///     to this event by resubmitting all sample requests that failed
        ///     while the device was in the process of changing.
        /// </member>
        MEStreamSinkDeviceChanged = 310,

        //
        // MEStreamTick
        // Stream Sinks can receive this event via IMFStreamSink::PlaceMarker.
        // This tells the Stream Sink to expect a gap in the data until at least
        // the event's PROPVARIANT value, which is given in terms of
        // Presentation Clock time.
        // In addition to optionally acting on this information, Stream Sinks
        // should handle this marker like any other marker.
        //

        //
        // MEPolicySet
        // Stream Sinks that are Output Trust Authorities and set policy
        // asynchronously should send this event.
        // This event is described in the "Trust events" section below.
        //

        //
        // MENetQualityReport
        // Sent by Stream Sinks that send data out over the network to provide
        // feedback on streaming
        // This event is described in the "Media Session events" section above
        //

        /// <member name="MEQualityNotify">
        ///     This event is generated by components which want to give a 
        ///     continuous feedback 
        ///     <para>
        ///         GUID: MF_QUALITY_NOTIFY_SAMPLE_LAG
        ///         Value: VT_I8
        ///         This event is generated by components which want to give a 
        ///         continuous feedback on whether things are going fine or bad. 
        ///         <para>
        ///             Value representing 100-nanosecond units indicating the 
        ///             lag time for the sample. Positive values show by how much
        ///             the component determined the sample to be late. Negative 
        ///             values show by how much the sample was delivered early
        ///         </para>
        ///     </para>
        ///
        ///     <para>
        ///         GUID: MF_QUALITY_NOTIFY_PROCESSING_LATENCY
        ///         Value: VT_I8
        ///         This event is generated by components which want to notify
        ///         the Quality Manager of their processing latency.   
        ///         <para>
        ///            Value represents a LONGLONG representating 100-nonosecond
        ///             unit value indicating latency. 
        ///
        ///             In order to determine that a sample is late the 
        ///             Quality Manager needs to know how much latency the 
        ///             component adds to delivering the sample. For example, on 
        ///             the EVR the Quality Manager knows when a sample is received
        ///             but has no good way of estimating the time it takes for 
        ///             this sample to eventually be presented on the screen. This
        ///             latency could vary depending on hardware.
        ///
        ///             It is also possible that there is no 1 to 1 mapping between
        ///             each sample received and the samples presented. For this
        ///             reason, just looking at process input and process output calls 
        ///             on the component to estimate the latency will not help. The
        ///             component is at the best position to determine this latency
        ///             and it should give the quality manager this information.
        ///
        ///             The component can update the processing latency anytime it 
        ///             thinks that it has changed by calling this function multiple times.
        ///         </para>
        ///     </para>    
        /// </member>
        MEQualityNotify = 311,

        /// <member name="MESinkInvalidated">
        ///     This event is generated by mediasession in the response to following events from the sink
        ///        MEAudioSessionFormatChanged,
        ///        MEAudioSessionDeviceRemoved,
        ///        MEAudioSessionServerShutdown,
        ///     Sink can also generate this event by itself. In this case, session simply propagates event to the application
        ///     <para>
        MESinkInvalidated = 312,

        /// <member name="MEAudioSessionNameChanged">
        ///     Indicates that the name of the audio session has changed.
        ///     Sent by the Streaming Audio Renderer Media Sink and forwarded
        ///     to the application by the Media Session
        /// </member>
        MEAudioSessionNameChanged = 313,

        /// <member name="MEAudioSessionVolumeChanged">
        ///     Indicates that the volume level has changed.
        ///     Sent by the Streaming Audio Renderer Media Sink and forwarded
        ///     to the application by the Media Session
        /// </member>
        MEAudioSessionVolumeChanged = 314,

        /// <member name="MEAudioSessionDeviceRemoved">
        ///     Indicates that the audio device through which playback had been
        ///     occurring has been removed.
        ///     Sent by the Streaming Audio Renderer Media Sink and forwarded
        ///     to the application by the Media Session
        /// </member>
        MEAudioSessionDeviceRemoved = 315,

        /// <member name="MEAudioSessionServerShutdown">
        ///     Indicates that the audio service on the machine has been shutdown.
        ///     Sent by the Streaming Audio Renderer Media Sink and forwarded
        ///     to the application by the Media Session
        /// </member>
        MEAudioSessionServerShutdown = 316,

        /// <member name="MEAudioSessionGroupingParamChanged">
        ///     Indicates that the audio session grouping parameters have changed.
        /// </member>
        MEAudioSessionGroupingParamChanged = 317,

        /// <member name="MEAudioSessionIconChanges">
        ///     Indicates that the audio session icon has changed.
        /// </member>
        MEAudioSessionIconChanged = 318,

        /// <member name="MEAudioSessionFormatChanged">
        ///     Indicates that the audio session format has changed.
        /// </member>
        MEAudioSessionFormatChanged = 319,

        /// <member name="MEAudioSessionDisconnected">
        ///     Indicates that the audio session was disconnected.
        /// </member>
        MEAudioSessionDisconnected = 320,

        /// <member name="MEAudioSessionExclusiveModeOverride">
        ///     Indicates that the audio session was pre-empted by an exclusive mode client.
        /// </member>
        MEAudioSessionExclusiveModeOverride = 321,

        /// <member name="MESinkV1Anchor">
        ///     Last source event for v1.  Do not add or remove events above this one.
        /// </member>
        MESinkV1Anchor = MEAudioSessionExclusiveModeOverride,

        /// <member name="MECaptureAudioSessionVolumeChanged">
        ///     Indicates that the capture volume level has changed.
        ///     Sent by the Streaming Audio Capture Media Source and forwarded
        ///     to the application by the Media Session
        /// </member>
        [SupportedOSPlatform("windows6.2")] // Windows 8
        MECaptureAudioSessionVolumeChanged = 322,

        /// <member name="MECaptureAudioSessionDeviceRemoved">
        ///     Indicates that the audio device through which capture had been
        ///     occurring has been removed.
        ///     Sent by the Streaming Audio Capture Media Source and forwarded
        ///     to the application by the Media Session
        /// </member>
        [SupportedOSPlatform("windows6.2")] // Windows 8
        MECaptureAudioSessionDeviceRemoved = 323,

        /// <member name="MECaptureAudioSessionFormatChanged">
        ///     Indicates that the capture audio session format has changed.
        /// </member>
        [SupportedOSPlatform("windows6.2")] // Windows 8
        MECaptureAudioSessionFormatChanged = 324,

        /// <member name="MECaptureAudioSessionDisconnected">
        ///     Indicates that the capture audio session was disconnected.
        /// </member>
        [SupportedOSPlatform("windows6.2")] // Windows 8
        MECaptureAudioSessionDisconnected = 325,

        /// <member name="MECaptureAudioSessionExclusiveModeOverride">
        ///     Indicates that the capture audio session was pre-empted by an exclusive mode client.
        /// </member>
        [SupportedOSPlatform("windows6.2")] // Windows 8
        MECaptureAudioSessionExclusiveModeOverride = 326,

        /// <member name="MECaptureAudioSessionServerShutdown">
        ///     Indicates that the capture audio session was disconnected due to the audio server beign shutdown.
        /// </member>
        [SupportedOSPlatform("windows6.2")] // Windows 8
        MECaptureAudioSessionServerShutdown = 327,

        /// <member name="MESinkV2Anchor">
        ///     Last source event for v2.  Do not add or remove events above this one.
        /// </member>
        [SupportedOSPlatform("windows6.2")] // Windows 8
        MESinkV2Anchor = MECaptureAudioSessionServerShutdown,

        //-------------------------------------------------------------------------
        // Trust Authority events:
        // Events of interest to IMFInputTrustAuthority and IMFTrustedOutput
        // implementations
        //-------------------------------------------------------------------------

        /// <member name="METrustUnknown">
        ///     Unknown event type.  Do not use for legitimate events.
        /// </member>
        METrustUnknown = 400,

        /// <member name="MEPolicyChanged">
        ///     All trusted outputs must handle this event, which describes
        ///     the new output policy that must be enforced.
        ///     For transforms, the event will come through
        ///     IMFTransform::ProcessInput.
        ///     For Media Sinks, it will come through IMFStreamSink::PlaceMarker.
        ///     The component should handle the new policy or return
        ///     MF_E_POLICY_UNSUPPORTED either synchronously or
        ///     asynchronously on the MEPolicyError event
        ///     Value:
        ///         VT_UNKNOWN
        ///         IMFOutputPolicy * describing the new output policy
        /// </member>
        MEPolicyChanged = 401,

        /// <member name="MEContentProtectionMessage">
        ///     All trusted outputs must handle this event, which carries a message
        ///     specific to the output policy already in use.
        ///     For transforms, the event will come through
        ///     IMFTransform::ProcessInput.
        ///     For Media Sinks, it will come through IMFStreamSink::PlaceMarker.
        ///     The component should handle the new policy or return
        ///     MF_E_POLICY_UNSUPPORTED either synchronously or
        ///     asynchronously on the MEPolicyError event
        ///     Value/Attributes:
        ///     [Event data is protection-system-specific]
        /// </member>
        MEContentProtectionMessage = 402,

        /// <member name="MEPolicySet">
        ///     Any Stream Sink that provides an IMFOutputTrustAuthority that
        ///     sets policy asynchronously should return MF_S_WAIT_FOR_POLICY_SET
        ///     from IMFOutputTrustAuthority::SetPolicy and send this event
        ///     when the policy has been set.
        /// </member>
        MEPolicySet = 403,

        /// <member name="METrustV1Anchor">
        ///     Last trust event for v1.  Do not add or remove events above this one.
        /// </member>
        METrustV1Anchor = MEPolicySet,

        //
        // MEPolicyError
        // Sent by any trusted component when an error is encountered enforcing
        // the required content protection policy.
        // This event is forwarded to the application by the Media Session.
        // This event is described in the "Media Session events" section above.
        //

        //
        // MEPolicyReport
        // Sent by any trusted output to report on how the content protection
        // system is being applied.
        // This event is forwarded to the application by the Media Session.
        // This event is described in the "Media Session events" section above.
        //

        //
        // MEEnablerProgress
        // Sent by the Input Trust Authority's IMFContentEnabler implementation.
        // This event is described in the "Media Session events" section above
        //

        //
        // MEEnablerCompleted
        // Sent by the Input Trust Authority's IMFContentEnabler implementation.
        // This event is described in the "Media Session events" section above
        //


        //-------------------------------------------------------------------------
        // WMDRM events:
        // Events of interest to applications using the Windows Media Digital
        // Rights Management (IWMDRMxxx) interfaces
        //-------------------------------------------------------------------------

        /// <member name="MEWMDRMLicenseBackupCompleted">
        ///     Sent by IWMDRMLicenseManagement
        ///     Value:
        ///         VT_UNKNOWN
        ///         IWMDRMLicenseBackupRestoreStatus *
        /// </member>
        MEWMDRMLicenseBackupCompleted = 500,

        /// <member name="MEWMDRMLicenseBackupProgress">
        ///     Sent by IWMDRMLicenseManagement
        ///     Value:
        ///         VT_UNKNOWN
        ///         IWMDRMLicenseBackupRestoreStatus *
        /// </member>
        MEWMDRMLicenseBackupProgress = 501,

        /// <member name="MEWMDRMLicenseRestoreCompleted">
        ///     Sent by IWMDRMLicenseManagement
        ///     Value:
        ///         VT_UNKNOWN
        ///         IWMDRMLicenseBackupRestoreStatus *
        /// </member>
        MEWMDRMLicenseRestoreCompleted = 502,

        /// <member name="MEWMDRMLicenseRestoreProgress">
        ///     Sent by IWMDRMLicenseManagement
        ///     Value:
        ///         VT_UNKNOWN
        ///         IWMDRMLicenseBackupRestoreStatus *
        /// </member>
        MEWMDRMLicenseRestoreProgress = 503,

        /// <member name="MEWMDRMLicenseAcquisitionCompleted">
        ///     Sent by IWMDRMLicenseManagement
        /// </member>
        MEWMDRMLicenseAcquisitionCompleted = 506,

        /// <member name="MEWMDRMIndividualizationCompleted">
        ///     Sent by IWMDRMSecurity
        ///     Value:
        ///         VT_UNKNOWN
        ///         IWMDRMIndividualizationStatus
        /// </member>
        MEWMDRMIndividualizationCompleted = 508,

        /// <member name="MEWMDRMIndividualizationProgress">
        ///     Sent by IWMDRMSecurity
        ///     Value:
        ///         VT_UNKNOWN:
        ///         IWMDRMIndividualizationStatus
        /// </member>
        MEWMDRMIndividualizationProgress = 513,

        /// <member name="MEWMDRMProximityCompleted">
        ///     Sent by IWMDRMNetReceiver
        /// </member>
        MEWMDRMProximityCompleted = 514,

        /// <member name="MEWMDRMLicenseStoreCleaned">
        ///     Sent by IWMDRMLicenseManagement
        /// </member>
        MEWMDRMLicenseStoreCleaned = 515,

        /// <member name="MEWMDRMRevocationDownloadCompleted">
        ///     Sent by IWMDRMSecurity
        /// </member>
        MEWMDRMRevocationDownloadCompleted = 516,

        /// <member name="MEWMDRMV1Anchor">
        ///     Last WMDRM event for v1.  Do not add or remove events above this one.
        /// </member>
        MEWMDRMV1Anchor = MEWMDRMRevocationDownloadCompleted,

        //-------------------------------------------------------------------------
        // MF Transform events:
        // Events sent by asynchronous IMFTransforms.
        //-------------------------------------------------------------------------

        /// <member name="METransformUnknown">
        ///     Unknown event type.  Do not use for legitimate events.
        /// </member>
        [SupportedOSPlatform("windows6.1")] // Windows 7
        METransformUnknown = 600,

        /// <member name="METransformNeedInput">
        ///     Asynchronous transforms send this event when they need another
        ///     call to IMFTransform::ProcessInput.  Callers should respond to
        ///     each such event by calling ProcessInput once.
        ///     Attributes:
        ///     MF_EVENT_MFT_INPUT_STREAM_ID:
        ///         This value should be the dwInputStreamID argument for the call
        ///         to ProcessInput
        /// </member>
        [SupportedOSPlatform("windows6.1")] // Windows 7
        METransformNeedInput,

        /// <member name="METransformHaveOutput">
        ///     Asynchronous transforms send this event when an output sample
        ///     becomes available.  Callers should respond to each such event by 
        ///     calling ProcessOutput once to pick up the sample.
        /// </member>
        [SupportedOSPlatform("windows6.1")] // Windows 7
        METransformHaveOutput,

        /// <member name="METransformDrainComplete">
        ///     Asynchronous transforms send this event after sending the final
        ///     METransformHaveOutput while executing a drain command.
        ///     Attributes:
        ///     MF_EVENT_MFT_CONTEXT:
        ///         This value corresponds to the value of ulParam that was 
        ///         specified in the ProcessMessage call for 
        ///         MFT_MESSAGE_COMMAND_DRAIN, which is the index of the input
        ///         stream for which the drain was requested.
        /// </member>
        [SupportedOSPlatform("windows6.1")] // Windows 7
        METransformDrainComplete,

        /// <member name="METransformMarker">
        ///     Asynchronous transforms send this event after receiving an 
        ///     MFT_MESSAGE_COMMAND_MARKER command and queueing 
        ///     METransformHaveOutput events for all outputs that will be produced
        ///     given the inputs received up until the command.
        ///     MF_EVENT_MFT_INPUT_STREAM_ID:
        ///         This value corresponds to the value of ulParam that was 
        ///         specified in the ProcessMessage call for 
        ///         MFT_MESSAGE_COMMAND_MARKER and can be used by the caller to
        ///         identify the marker.
        /// </member>
        [SupportedOSPlatform("windows6.1")] // Windows 7
        METransformMarker,

        /// <member name="METransformInputStreamStateChanged">
        ///     Asynchronous transforms send this event when they want a specific input
        ///     stream to be placed in a stop/pause/run/disabled state. 
        ///     Attributes:
        ///     MF_EVENT_MFT_CONTEXT:
        /// </member>
        [SupportedOSPlatform("windows6.2")] // Windows 8
        METransformInputStreamStateChanged,

        //-------------------------------------------------------------------------
        // IMFByteStream events:
        // Events sent by implementations of IMFByteStream.
        //-------------------------------------------------------------------------

        /// <member name="MEByteStreamCharacteristicsChanged">
        ///     Indicates that the values returned by the IMFByteStream::GetCapabilities,
        ///     IMFByteStream::GetLength and IMFByteStream::IsEndOfStream may have
        ///     changed.
        /// </member>
        [SupportedOSPlatform("windows6.2")] // Windows 8
        MEByteStreamCharacteristicsChanged = 700,

        //-------------------------------------------------------------------------
        // Device Source events:
        // Events sent by IMFMediaSource that encapsulates a device.
        //-------------------------------------------------------------------------

        /// <member name="MEVideoCaptureDeviceRemoved">
        ///     Indicates that the device represented by the camera device has been
        ///     removed from the system
        /// </member>
        [SupportedOSPlatform("windows6.2")] // Windows 8
        MEVideoCaptureDeviceRemoved = 800,
        [SupportedOSPlatform("windows6.2")] // Windows 8
        MEVideoCaptureDevicePreempted = 801,

        //-------------------------------------------------------------------------
        // Media Session/Media Sink events:
        // Events sent by media sinks and consumed by the Media Session or the application
        //-------------------------------------------------------------------------

        /// <member name="MEStreamSinkFormatInvalidated">
        ///     Sent by a Stream Sink when the downstream format has become invalidated
        ///     in such a way that it needs to be renegotiated (and any data that was queued
        ///     to the sink past current playback position should be resent.
        /// </member>
        [SupportedOSPlatform("windows6.2")] // Windows 8
        MEStreamSinkFormatInvalidated = 802,

        //-------------------------------------------------------------------------
        // Media Foundation Transforms events:
        // Events sent by the media pipelines (e.g. SinkWriter) and consumed by MFTs
        //-------------------------------------------------------------------------

        /// <member name="MEEncodingParameters">
        ///     Sent by the pipeline to encoder MFTs serially with media samples (via IMFTransform::ProcessEvent)
        ///     Event payload is an attribute store (IMFAttributes pointer) that contains the new ICodecAPI-based
        ///     settings that the encoder should apply on subsequent incoming samples.
        /// </member>
        [SupportedOSPlatform("windows6.2")] // Windows 8
        MEEncodingParameters = 803,

        /// <member name="MEContentProtectionMetadata">
        ///     Media Stream uses this event to send protection system specific 
        ///     metadata to the decoder. It is used for e.g. communicating key rotation
        ///     event and in this case should be send as early as possible to give decoder time 
        ///     to prepare itself before sample encrypted with new key ID start arriving.
        ///     Attributes:
        ///         MF_EVENT_STREAM_METADATA_KEYDATA
        ///             Value: BLOB
        ///             This is an optional attribute. Protection system specific data.
        ///         MF_EVENT_STREAM_METADATA_CONTENT_KEYIDS
        ///             Value: BLOB
        ///             Content key IDs which the event is associated with.
        ///         MF_EVENT_STREAM_METADATA_SYSTEMID
        ///             Value: BLOB
        ///             This is an optional attribute. System ID for which the key data is intended.
        /// </member>
        // _WIN32_WINNT_WINBLUE means Windows 6.3 which it does correspond to Windows 8.1.
        // Interesting...
        [SupportedOSPlatform("windows6.3")] // Windows 8.1
        MEContentProtectionMetadata = 900,


        /// <member name="MEDeviceThermalStateChanged">
        ///     This event is to notify the MF pipeline components 
        ///     when a device thermal state changes
        ///     Value:
        ///         VT_UINT
        ///         Description: This value corresponds to the current thermal value reported by the device in percentage.
        /// </member>
        // TODO: Does _WIN32_WINNT_WINTHRESHOLD mean: The Windows version before 10?
        // If not we must update the following field appropriately.
        [SupportedOSPlatform("windows6.3")] // Windows 8.1
        MEDeviceThermalStateChanged = 950,

        //-------------------------------------------------------------------------
        // MF reserves a range of events for internal and future use
        //-------------------------------------------------------------------------

        /// <member name="MEReservedMax">
        ///     All event type codes up to and including this value are
        ///     reserved.
        /// </member>
        MEReservedMax = 10000,


    }
}